using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FieldBank.Infrastructure.Extensions;
using FieldBank.Application.Extensions;
using FieldBank.Infrastructure.Caching;
using MediatR;
using Serilog;

namespace FieldBank.Functions;

/// <summary>
/// Initializes the Generic Host for all Lambdas,
/// registering DbContext, repositories, application services, and caching.
/// </summary>
public static class FunctionsStartup
{
    public static IHostBuilder ConfigureServices(this IHostBuilder builder)
    {
        // Early startup log (optional)
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        Log.Information("Serilog is working in startup!");

        return builder
            .UseSerilog((context, services, configuration) =>
            {
                var environment = context.HostingEnvironment.EnvironmentName;
                var isDevelopment = environment.Equals("Development", StringComparison.OrdinalIgnoreCase);

                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithThreadId()
                    .Enrich.WithProcessId()
                    .Enrich.WithEnvironmentUserName()
                    .Enrich.WithProperty("Application", "FieldBank")
                    .Enrich.WithProperty("Environment", environment);

                // Console sink
                configuration.WriteTo.Console(
                    outputTemplate: isDevelopment
                        ? "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
                        : "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

                // File sink only in development
                if (isDevelopment)
                {
                    configuration.WriteTo.File(
                        path: "logs/fieldbank-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
                }
            })
            .ConfigureServices((context, services) =>
            {
                services.AddFieldBankDbContext(context.Configuration);
                services.AddFieldBankInfrastructure();
                services.AddFieldBankApplicationServices();
                services.AddSingleton<ICacheService, RedisCacheService>();
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Features.Fields.Commands.CreateField.CreateFieldCommand).Assembly));
                services.AddLogging();
            });
    }
}

