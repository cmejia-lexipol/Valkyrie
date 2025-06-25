using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FieldBank.Infrastructure.Extensions;
using FieldBank.Application.Extensions;
using MediatR;
using Serilog;
using Serilog.Extensions.Logging;

namespace FieldBank.Functions;

/// <summary>
/// Helper class to centralize test configuration for AWS Lambda Test Tool
/// </summary>
public static class TestConfigurationHelper
{
    /// <summary>
    /// Creates a configured service provider for testing
    /// </summary>
    /// <returns>Configured service provider</returns>
    public static IServiceProvider CreateTestServiceProvider()
    {
        var services = new ServiceCollection();

        // Initialize Serilog for console and file
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                path: "logs/fieldbank-test-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .CreateLogger();

        // Create configuration with hardcoded connection string for test tool
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=FieldBank;Username=postgres;Password=password"
            })
            .Build();

        // Register services
        services.AddFieldBankDbContext(configuration);
        services.AddFieldBankInfrastructure();
        services.AddFieldBankApplicationServices();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Features.Fields.Commands.CreateField.CreateFieldCommand).Assembly));

        // Add Serilog provider to Microsoft logging
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(Log.Logger, dispose: true);
        });

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Gets a service from the test service provider
    /// </summary>
    /// <typeparam name="T">Service type</typeparam>
    /// <returns>Service instance</returns>
    public static T GetTestService<T>() where T : notnull
    {
        var serviceProvider = CreateTestServiceProvider();
        return serviceProvider.GetRequiredService<T>();
    }
}