using Amazon.Lambda.Core;
using MediatR;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Valkyrie.Infrastructure.Extensions;
using Valkyrie.Application.Extensions;

namespace Valkyrie.Functions.Handlers;

public class GetCategoriesFunction
{
    private readonly IMediator _mediator;

    // Constructor for dependency injection (used by your app)
    public GetCategoriesFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Parameterless constructor for AWS Lambda (production)
    public GetCategoriesFunction()
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
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Features.Categories.Commands.CreateCategory.CreateCategoryCommand).Assembly));
                services.AddLogging();
            })
            .Build();

        _mediator = host.Services.GetRequiredService<IMediator>();
    }

    /// <summary>
    /// Lambda function handler for getting all categories
    /// </summary>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(ILambdaContext context)
    {
        context.Logger.LogInformation("Getting all categories");

        try
        {
            var categories = await _mediator.Send(new Application.Features.Categories.Queries.GetAllCategories.GetAllCategoriesQuery());
            context.Logger.LogInformation($"Retrieved {categories.Count()} categories");
            
            return JsonSerializer.Serialize(categories);
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error getting categories: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
} 