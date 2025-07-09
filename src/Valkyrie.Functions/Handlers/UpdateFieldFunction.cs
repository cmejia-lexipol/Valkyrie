using Amazon.Lambda.Core;
using MediatR;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Valkyrie.Functions.Handlers;

public class UpdateFieldFunction : LambdaHandlerBase
{
    public UpdateFieldFunction(IMediator mediator) : base(mediator) { }
    public UpdateFieldFunction() : base() { }

    /// <summary>
    /// Lambda function handler for updating a field
    /// </summary>
    /// <param name="request">The request containing field data to update</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(UpdateFieldRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Updating field with ID: {request.Id}");

        try
        {
            var field = await _mediator.Send(new Application.Features.Fields.Commands.UpdateField.UpdateFieldCommand
            {
                Id = request.Id,
                Name = request.Name,
                Label = request.Label,
                Description = request.Description,
                CategoryId = request.CategoryId
            });

            context.Logger.LogInformation($"Updated field: {field.Name}");
            return JsonSerializer.Serialize(field);
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
            context.Logger.LogError($"Error updating field: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
}

public class UpdateFieldRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
}