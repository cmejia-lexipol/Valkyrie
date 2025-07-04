using Amazon.Lambda.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Functions.Handlers;

public class GetFieldTypesFunction
{
    private readonly IMediator _mediator;

    // Constructor for dependency injection (used by your app)
    public GetFieldTypesFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Parameterless constructor for AWS Lambda (production)
    public GetFieldTypesFunction()
    {
        var host = FunctionsStartup.BuildHost();
        _mediator = host.Services.GetRequiredService<IMediator>();
    }

    /// <summary>
    /// Lambda function handler for getting all field types
    /// </summary>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<IEnumerable<FieldTypeDto>> FunctionHandler(ILambdaContext context)
    {
        context.Logger.LogInformation("Getting all field types");

        try
        {
            var fieldTypes = await _mediator.Send(new Application.Features.FieldTypes.Queries.GetAllFieldTypes.GetAllFieldTypesQuery());
            context.Logger.LogInformation($"Retrieved {fieldTypes.Count()} field types");
            return fieldTypes;
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error getting field types: {ex.Message}");
            throw;
        }
    }
} 