using Amazon.Lambda.Core;
using MediatR;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Functions.Handlers;

public class GetCategoriesFunction : LambdaHandlerBase
{
    public GetCategoriesFunction(IMediator mediator) : base(mediator) { }
    public GetCategoriesFunction() : base() { }

    /// <summary>
    /// Lambda function handler for getting all categories
    /// </summary>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<IEnumerable<CategoryDto>> FunctionHandler(ILambdaContext context)
    {
        context.Logger.LogInformation("Getting all categories");

        try
        {
            var categories = await _mediator.Send(new Application.Features.Categories.Queries.GetAllCategories.GetAllCategoriesQuery());
            context.Logger.LogInformation($"Retrieved {categories.Count()} categories");

            return categories;
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error getting categories: {ex.Message}");
            throw;
        }
    }
}