using Microsoft.Extensions.DependencyInjection;
using Valkyrie.Application.Common.Mappings;

namespace Valkyrie.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValkyrieApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddSingleton<FieldMapper>();
        services.AddSingleton<CategoryMapper>();
        services.AddSingleton<FieldTypeMapper>();
        services.AddSingleton<FieldCommandMapper>();
        services.AddSingleton<CategoryCommandMapper>();

        return services;
    }
}