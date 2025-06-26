using Amazon.Lambda.Core;
using MediatR;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Valkyrie.Infrastructure.Extensions;
using Valkyrie.Application.Extensions;

namespace Valkyrie.Functions.Handlers;

public class UpdateFieldFunction
{
    private readonly IMediator _mediator;

    // Constructor for dependency injection (used by your app)
    public UpdateFieldFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Parameterless constructor for AWS Lambda (production)
    public UpdateFieldFunction()
    {
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