using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Valkyrie.Infrastructure.Persistence;
using Valkyrie.Infrastructure.Repositories;
using Valkyrie.Domain.Interfaces;
using Valkyrie.Application.Extensions;

namespace Valkyrie.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValkyrieDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ValkyrieDBContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(ValkyrieDBContext).Assembly.FullName)
            ));

        return services;
    }

    public static IServiceCollection AddValkyrieRepositories(this IServiceCollection services)
    {
        // Generic repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Specific repositories
        services.AddScoped<IFieldRepository, FieldRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IFieldTypeRepository, FieldTypeRepository>();

        return services;
    }

    public static IServiceCollection AddValkyrieInfrastructure(this IServiceCollection services)
    {
        // Register repositories
        services.AddValkyrieRepositories();

        return services;
    }
}
