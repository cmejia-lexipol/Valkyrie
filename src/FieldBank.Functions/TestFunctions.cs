using Amazon.Lambda.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FieldBank.Functions;

public class TestFunctions
{
    private readonly IMediator _mediator;

    // Parameterless constructor for Lambda test tool
    public TestFunctions()
    {
        // Use centralized test configuration helper
        _mediator = TestConfigurationHelper.GetTestService<IMediator>();
    }

    /// <summary>
    /// Unified Lambda function handler for testing all CRUD operations
    /// </summary>
    /// <param name="request">The test request containing operation and data</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(TestRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Executing operation: {request.Operation}");

        try
        {
            switch (request.Operation?.ToLower())
            {
                case "getall":
                    return await HandleGetAll(context);

                case "getbyid":
                    if (!request.Id.HasValue)
                        return "ID is required for getbyid operation";
                    return await HandleGetById(request.Id.Value, context);

                case "create":
                    return await HandleCreate(
                        request.Name ?? "Default Name", 
                        request.Label ?? "Default Label", 
                        request.Description, 
                        context);

                case "update":
                    if (!request.Id.HasValue)
                        return "ID is required for update operation";
                    return await HandleUpdate(
                        request.Id.Value, 
                        request.Name ?? "Default Name", 
                        request.Label ?? "Default Label", 
                        request.Description, 
                        context);

                case "delete":
                    if (!request.Id.HasValue)
                        return "ID is required for delete operation";
                    return await HandleDelete(request.Id.Value, context);

                default:
                    return "Invalid operation. Use 'getall', 'getbyid', 'create', 'update', or 'delete'";
            }
        }
        catch (ArgumentException ex)
        {
            context.Logger.LogError($"Validation error: {ex.Message}");
            return $"Validation error: {ex.Message}";
        }
        catch (InvalidOperationException ex)
        {
            context.Logger.LogError($"Business logic error: {ex.Message}");
            return $"Business logic error: {ex.Message}";
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error processing request: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }

    private async Task<string> HandleGetAll(ILambdaContext context)
    {
        context.Logger.LogInformation("Getting all fields");
        var fields = await _mediator.Send(new Application.Features.Fields.Queries.GetAllFields.GetAllFieldsQuery());
        context.Logger.LogInformation($"Retrieved {fields.Count()} fields");
        return JsonSerializer.Serialize(fields);
    }

    private async Task<string> HandleGetById(int id, ILambdaContext context)
    {
        context.Logger.LogInformation($"Getting field with ID: {id}");
        var field = await _mediator.Send(new Application.Features.Fields.Queries.GetFieldById.GetFieldByIdQuery { Id = id });
        
        if (field != null)
        {
            context.Logger.LogInformation($"Retrieved field: {field.Name}");
            return JsonSerializer.Serialize(field);
        }
        else
        {
            context.Logger.LogWarning($"Field with ID {id} not found");
            return "Field not found";
        }
    }

    private async Task<string> HandleCreate(string name, string label, string? description, ILambdaContext context)
    {
        context.Logger.LogInformation($"Creating field: {name}");
        var field = await _mediator.Send(new Application.Features.Fields.Commands.CreateField.CreateFieldCommand
        {
            Name = name,
            Label = label,
            Description = description
        });
        context.Logger.LogInformation($"Created field with ID: {field.Id}");
        return JsonSerializer.Serialize(field);
    }

    private async Task<string> HandleUpdate(int id, string name, string label, string? description, ILambdaContext context)
    {
        context.Logger.LogInformation($"Updating field with ID: {id}");
        var field = await _mediator.Send(new Application.Features.Fields.Commands.UpdateField.UpdateFieldCommand
        {
            Id = id,
            Name = name,
            Label = label,
            Description = description
        });
        context.Logger.LogInformation($"Updated field: {field.Name}");
        return JsonSerializer.Serialize(field);
    }

    private async Task<string> HandleDelete(int id, ILambdaContext context)
    {
        context.Logger.LogInformation($"Deleting field with ID: {id}");
        await _mediator.Send(new Application.Features.Fields.Commands.DeleteField.DeleteFieldCommand { Id = id });
        context.Logger.LogInformation($"Deleted field with ID: {id}");
        return "Field deleted successfully";
    }
}

// Request model for testing all operations
public class TestRequest
{
    public string? Operation { get; set; }
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }
} 