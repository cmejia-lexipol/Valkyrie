using Amazon.Lambda.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Valkyrie.Functions.Handlers;

public class CreateCategoryFunction : LambdaHandlerBase
{
    public CreateCategoryFunction(IMediator mediator) : base(mediator) { }
    public CreateCategoryFunction() : base() { }

    public async Task<string> FunctionHandler(CreateCategoryRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Creating Category: {request.Name}");


        try
        {
            var category = await _mediator.Send(new Application.Features.Categories.Commands.CreateCategory.CreateCategoryCommand
            {
                Name = request.Name,
                Description = request.Description,
                Rank = request.Rank

            });

            context.Logger.LogInformation($"Created category with ID: {category.CategoryId}");
            return JsonSerializer.Serialize(category);
        }
        catch (ArgumentException ex)
        {
            context.Logger.LogError($"Validation error: {ex.Message}");
            return $"Validation error: {ex.Message}";
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error creating Category: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
}

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Rank { get; set; }
}