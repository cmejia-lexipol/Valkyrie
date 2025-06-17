using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using FieldBank.Infrastructure.Extensions;
using FieldBank.Infrastructure.Repositories;
using FieldBank.Infrastructure.Caching;

namespace FieldBank.Functions
{
    /// <summary>
    /// Esta clase se encarga de inicializar el Generic Host para todas las Lambdas,
    /// registrando DbContext, repositorios y caching.
    /// </summary>
    public static class FunctionsStartup
    {
        public static IHostBuilder ConfigureServices(this IHostBuilder builder)
        {
            return builder
                // Configuración de Serilog
                .UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.FromLogContext();
                })

                // Registro de servicios en el contenedor DI
                .ConfigureServices((context, services) =>
                {
                    // 1. EF Core con configuración centralizada
                    services.AddFieldBankDbContext(context.Configuration);

                    // 2. Repositorios genéricos
                    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

                    // 3. Cache service
                    services.AddSingleton<ICacheService, RedisCacheService>();
                });
        }
    }
}
