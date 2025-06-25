using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FieldBank.Infrastructure.Persistence;
using FieldBank.Infrastructure.Repositories;
using FieldBank.Domain.Interfaces;
using FieldBank.Application.Extensions;

namespace FieldBank.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFieldBankDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<FieldBankDBContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(FieldBankDBContext).Assembly.FullName)
            ));

        return services;
    }

    public static IServiceCollection AddFieldBankRepositories(this IServiceCollection services)
    {
        // Generic repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Specific repositories
        services.AddScoped<IFieldRepository, FieldRepository>();

        return services;
    }

    public static IServiceCollection AddFieldBankInfrastructure(this IServiceCollection services)
    {
        // Register repositories
        services.AddFieldBankRepositories();

        return services;
    }
}
