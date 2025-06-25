using Amazon.Lambda.Core;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FieldBank.Infrastructure.Extensions;
using FieldBank.Application.Extensions;

namespace FieldBank.Functions.Handlers;

public class DeleteFieldFunction
{
    private readonly IMediator _mediator;

    // Constructor for dependency injection (used by your app)
    public DeleteFieldFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Parameterless constructor for AWS Lambda (production)
    public DeleteFieldFunction()
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
    /// Lambda function handler for deleting a field
    /// </summary>
    /// <param name="request">The request containing the field ID to delete</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(DeleteFieldRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Deleting field with ID: {request.Id}");

        try
        {
            await _mediator.Send(new Application.Features.Fields.Commands.DeleteField.DeleteFieldCommand
            {
                Id = request.Id
            });
            context.Logger.LogInformation($"Deleted field with ID: {request.Id}");
            return "Field deleted successfully";
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
            context.Logger.LogError($"Error deleting field: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
}

public class DeleteFieldRequest
{
    public int Id { get; set; }
}