using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace FieldBank.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFieldBankApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        return services;
    }
}