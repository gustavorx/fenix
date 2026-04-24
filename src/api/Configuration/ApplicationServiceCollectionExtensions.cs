using System.Reflection;
using api.Shared;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace api.Configuration;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationServiceCollectionExtensions).Assembly;

        RegisterUseCases(services, assembly);
        RegisterValidators(services, assembly);

        return services;
    }

    private static void RegisterUseCases(IServiceCollection services, Assembly assembly)
    {
        var useCaseTypes = assembly.DefinedTypes
            .Where(type =>
                type is { IsClass: true, IsAbstract: false } &&
                type.Name.EndsWith("UseCase", StringComparison.Ordinal))
            .Select(type => type.AsType());

        foreach (var useCaseType in useCaseTypes)
        {
            services.TryAddScoped(useCaseType);
        }
    }

    private static void RegisterValidators(IServiceCollection services, Assembly assembly)
    {
        var validatorRegistrations = assembly.DefinedTypes
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .SelectMany(type => type.ImplementedInterfaces
                .Where(IsValidatorInterface)
                .Select(@interface => new
                {
                    ServiceType = @interface,
                    ImplementationType = type.AsType()
                }));

        foreach (var registration in validatorRegistrations)
        {
            services.TryAddScoped(registration.ServiceType, registration.ImplementationType);
        }
    }

    private static bool IsValidatorInterface(Type type)
    {
        return type.IsGenericType &&
               type.GetGenericTypeDefinition() == typeof(IValidator<>);
    }
}
