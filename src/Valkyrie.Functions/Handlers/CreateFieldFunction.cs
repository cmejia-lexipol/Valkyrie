using Amazon.Lambda.Core;
using MediatR;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Valkyrie.Infrastructure.Extensions;
using Valkyrie.Application.Extensions;

namespace Valkyrie.Functions.Handlers;

public class CreateFieldFunction
{
    private readonly IMediator _mediator;

    // Constructor for dependency injection (used by your app)
    public CreateFieldFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Parameterless constructor for AWS Lambda (production)
    public CreateFieldFunction()
    {
        // Build the Generic Host using FunctionsStartup
        var host = new HostBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true)
                      .AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddValkyrieDbContext(context.Configuration);
                services.AddValkyrieInfrastructure();
                services.AddValkyrieApplicationServices();
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Features.Fields.Commands.CreateField.CreateFieldCommand).Assembly));
                services.AddLogging();
            })
            .Build();

        _mediator = host.Services.GetRequiredService<IMediator>();
    }

    /// <summary>
    /// Lambda function handler for creating a new field
    /// </summary>
    /// <param name="request">The request containing field data</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(CreateFieldRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Creating field: {request.Name}");

        try
        {
            var field = await _mediator.Send(new Application.Features.Fields.Commands.CreateField.CreateFieldCommand
            {
                Name = request.Name,
                Label = request.Label,
                Description = request.Description
            });

            context.Logger.LogInformation($"Created field with ID: {field.Id}");
            return JsonSerializer.Serialize(field);
        }
        catch (ArgumentException ex)
        {
            context.Logger.LogError($"Validation error: {ex.Message}");
            return $"Validation error: {ex.Message}";
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error creating field: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
}

public class CreateFieldRequest
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
}