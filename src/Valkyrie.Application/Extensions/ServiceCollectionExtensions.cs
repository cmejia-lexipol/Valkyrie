using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace Valkyrie.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValkyrieApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        return services;
    }
}