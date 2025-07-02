using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Valkyrie.Infrastructure.Extensions;
using Valkyrie.Application.Extensions;
using Valkyrie.Infrastructure.Caching;
using MediatR;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace Valkyrie.Functions;

/// <summary>
/// Initializes the Generic Host for all Lambdas,
/// registering DbContext, repositories, application services, and caching.
/// </summary>
public static class FunctionsStartup
{

    public static IHost BuildHost()
    {
        return new HostBuilder()
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
                    .Enrich.WithProperty("Application", "Valkyrie")
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
                        path: "logs/Valkyrie-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
                }
            })
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
                services.AddSingleton<ICacheService, RedisCacheService>();
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Features.Fields.Commands.CreateField.CreateFieldCommand).Assembly));
                services.AddLogging();
            })
            .ConfigureAppConfiguration((context, config) => { })
            .ConfigureHostConfiguration((config) => { })
            .ConfigureServices((context, services) =>
            {
                // Seed the database after building the host
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<Valkyrie.Infrastructure.Persistence.ValkyrieDBContext>();
                Valkyrie.Infrastructure.Persistence.SeedData.Seed(db);
            })
            .Build();
    }
}

