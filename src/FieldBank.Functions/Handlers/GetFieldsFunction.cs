using Amazon.Lambda.Core;
using MediatR;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FieldBank.Infrastructure.Extensions;
using FieldBank.Application.Extensions;

namespace FieldBank.Functions.Handlers;

public class GetFieldsFunction
{
    private readonly IMediator _mediator;

    // Constructor for dependency injection (used by your app)
    public GetFieldsFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Parameterless constructor for AWS Lambda (production)
    public GetFieldsFunction()
    {
        var host = new HostBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true)
                      .AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddFieldBankDbContext(context.Configuration);
                services.AddFieldBankInfrastructure();
                services.AddFieldBankApplicationServices();
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Features.Fields.Commands.CreateField.CreateFieldCommand).Assembly));
                services.AddLogging();
            })
            .Build();

        _mediator = host.Services.GetRequiredService<IMediator>();
    }

    /// <summary>
    /// Lambda function handler for getting all fields
    /// </summary>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(ILambdaContext context)
    {
        context.Logger.LogInformation("Getting all fields");

        try
        {
            var fields = await _mediator.Send(new Application.Features.Fields.Queries.GetAllFields.GetAllFieldsQuery());
            context.Logger.LogInformation($"Retrieved {fields.Count()} fields");

            return JsonSerializer.Serialize(fields);
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error getting fields: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
}