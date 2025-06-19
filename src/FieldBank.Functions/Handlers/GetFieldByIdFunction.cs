using Amazon.Lambda.Core;
using MediatR;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FieldBank.Infrastructure.Extensions;
using FieldBank.Application.Extensions;

namespace FieldBank.Functions.Handlers;

public class GetFieldByIdFunction
{
    private readonly IMediator _mediator;

    // Constructor for dependency injection (used by your app)
    public GetFieldByIdFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Parameterless constructor for AWS Lambda (production)
    public GetFieldByIdFunction()
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
    /// Lambda function handler for getting a field by ID
    /// </summary>
    /// <param name="request">The request containing the field ID</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(GetFieldByIdRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Getting field with ID: {request.Id}");

        try
        {
            var field = await _mediator.Send(new Application.Features.Fields.Queries.GetFieldById.GetFieldByIdQuery
            {
                Id = request.Id
            });
            
            if (field != null)
            {
                context.Logger.LogInformation($"Retrieved field: {field.Name}");
                return JsonSerializer.Serialize(field);
            }
            else
            {
                context.Logger.LogWarning($"Field with ID {request.Id} not found");
                return "Field not found";
            }
        }
        catch (ArgumentException ex)
        {
            context.Logger.LogError($"Validation error: {ex.Message}");
            return $"Validation error: {ex.Message}";
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error getting field: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
}

public class GetFieldByIdRequest
{
    public int Id { get; set; }
} 