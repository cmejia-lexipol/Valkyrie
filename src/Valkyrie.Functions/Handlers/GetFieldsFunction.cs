using Amazon.Lambda.Core;
using MediatR;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Functions.Handlers;

public class GetFieldsFunction : LambdaHandlerBase
{
    public GetFieldsFunction(IMediator mediator) : base(mediator) { }
    public GetFieldsFunction() : base() { }

    /// <summary>
    /// Lambda function handler for getting all fields
    /// </summary>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<IEnumerable<FieldDto>> FunctionHandler(ILambdaContext context)
    {
        context.Logger.LogInformation("Getting all fields");

        try
        {
            var fields = await _mediator.Send(new Application.Features.Fields.Queries.GetAllFields.GetAllFieldsQuery());
            context.Logger.LogInformation($"Retrieved {fields.Count()} fields");

            return fields;
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error getting fields: {ex.Message}");
            throw;
        }
    }
}