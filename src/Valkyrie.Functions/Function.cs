using Amazon.Lambda.Core;
using MediatR;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Valkyrie.Infrastructure.Extensions;
using Valkyrie.Application.Extensions;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Valkyrie.Functions;

public class Function
{
    private readonly IMediator _mediator;

    // Constructor for dependency injection (used by your app)
    public Function(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Parameterless constructor for Lambda test tool
    public Function()
    {
        var host = FunctionsStartup.BuildHost();
        _mediator = host.Services.GetRequiredService<IMediator>();
    }

    /// <summary>
    /// Lambda function handler for Field operations
    /// </summary>
    /// <param name="request">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(FieldRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Processing request: {JsonSerializer.Serialize(request ?? new FieldRequest())}");

        try
        {
            switch (request?.Operation?.ToLower())
            {
                case "create":
                    var createdField = await _mediator.Send(new Application.Features.Fields.Commands.CreateField.CreateFieldCommand
                    {
                        Name = request.Name ?? "Default Name",
                        Label = request.Label ?? "Default Label",
                        Description = request.Description,
                        CategoryId = request.CategoryId ?? 0
                    });

                    context.Logger.LogInformation($"Created field with ID: {createdField.FieldId}");
                    return JsonSerializer.Serialize(createdField);

                case "get":
                    if (request.Id.HasValue)
                    {
                        var field = await _mediator.Send(new Application.Features.Fields.Queries.GetFieldById.GetFieldByIdQuery
                        {
                            Id = request.Id.Value
                        });
                        if (field != null)
                        {
                            context.Logger.LogInformation($"Retrieved field: {field.Name}");
                            return JsonSerializer.Serialize(field);
                        }
                        else
                        {
                            return "Field not found";
                        }
                    }
                    return "ID is required for get operation";

                case "getall":
                    var allFields = await _mediator.Send(new Application.Features.Fields.Queries.GetAllFields.GetAllFieldsQuery());
                    context.Logger.LogInformation($"Retrieved {allFields.Count()} fields");
                    return JsonSerializer.Serialize(allFields);

                case "update":
                    if (request.Id.HasValue)
                    {
                        var updatedField = await _mediator.Send(new Application.Features.Fields.Commands.UpdateField.UpdateFieldCommand
                        {
                            Id = request.Id.Value,
                            Name = request.Name ?? "Default Name",
                            Label = request.Label ?? "Default Label",
                            Description = request.Description,
                            CategoryId = request.CategoryId ?? 0
                        });

                        context.Logger.LogInformation($"Updated field: {updatedField.Name}");
                        return JsonSerializer.Serialize(updatedField);
                    }
                    return "ID is required for update operation";

                case "delete":
                    if (request.Id.HasValue)
                    {
                        await _mediator.Send(new Application.Features.Fields.Commands.DeleteField.DeleteFieldCommand
                        {
                            Id = request.Id.Value
                        });
                        context.Logger.LogInformation($"Deleted field with ID: {request.Id.Value}");
                        return "Field deleted successfully";
                    }
                    return "ID is required for delete operation";

                default:
                    return "Invalid operation. Use 'create', 'get', 'getall', 'update', or 'delete'";
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
}

// Request model for the Lambda function
public class FieldRequest
{
    public string? Operation { get; set; }
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
}
