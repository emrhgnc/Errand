using System.Reflection;
using Errand.Abstractions;
using Errand.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Errand.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register Errand services.
/// "You have my sword, and my bow, and my service registration!" - Legolas & Gimli
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Errand services with the service collection.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblies">Assemblies to scan for handlers.  If none provided, uses calling assembly.</param>
    /// <param name="lifetime">Service lifetime for handlers (default:  Transient)</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddErrand(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient,
        params Assembly[] assemblies)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // Register the Errand mediator
        services.TryAddScoped<IErrand, Errand.Core.Errand>();

        // If no assemblies specified, use calling assembly
        if (assemblies.Length == 0)
        {
            assemblies = new[] { Assembly.GetCallingAssembly() };
        }

        // Register all handlers from specified assemblies
        RegisterHandlers(services, assemblies, lifetime);

        return services;
    }

    /// <summary>
    /// Registers Errand services with the service collection and scans specific types.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="handlerAssemblyMarkerTypes">Types from assemblies to scan for handlers</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddErrand(
        this IServiceCollection services,
        params Type[] handlerAssemblyMarkerTypes)
    {
        if (handlerAssemblyMarkerTypes.Length == 0)
        {
            throw new ArgumentException("At least one assembly marker type must be provided.", nameof(handlerAssemblyMarkerTypes));
        }

        var assemblies = handlerAssemblyMarkerTypes
            .Select(t => t.Assembly)
            .Distinct()
            .ToArray();

        return services.AddErrand(ServiceLifetime.Transient, assemblies);
    }

    /// <summary>
    /// Adds a pipeline behavior that will be executed for all requests.
    /// </summary>
    /// <typeparam name="TBehavior">The behavior type</typeparam>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddErrandBehavior<TBehavior>(this IServiceCollection services)
        where TBehavior : class
    {
        // Register as open generic if possible
        var behaviorType = typeof(TBehavior);

        if (behaviorType.IsGenericTypeDefinition)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), behaviorType);
        }
        else
        {
            var interfaces = behaviorType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>))
                .ToList();

            foreach (var @interface in interfaces)
            {
                services.AddTransient(@interface, behaviorType);
            }
        }

        return services;
    }

    /// <summary>
    /// Adds pipeline behaviors from the specified assemblies.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblies">Assemblies to scan for behaviors</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddErrandBehaviors(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            assemblies = new[] { Assembly.GetCallingAssembly() };
        }

        RegisterBehaviors(services, assemblies);
        return services;
    }

    private static void RegisterHandlers(
        IServiceCollection services,
        Assembly[] assemblies,
        ServiceLifetime lifetime)
    {
        var handlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Select(t => new
            {
                Type = t,
                Interfaces = t.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                               i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                    .ToList()
            })
            .Where(x => x.Interfaces.Any())
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            foreach (var @interface in handlerType.Interfaces)
            {
                services.Add(new ServiceDescriptor(@interface, handlerType.Type, lifetime));
            }
        }
    }

    private static void RegisterBehaviors(IServiceCollection services, Assembly[] assemblies)
    {
        var behaviorTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new
            {
                Type = t,
                Interfaces = t.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                               i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>))
                    .ToList()
            })
            .Where(x => x.Interfaces.Any())
            .ToList();

        foreach (var behaviorType in behaviorTypes)
        {
            if (behaviorType.Type.IsGenericTypeDefinition)
            {
                services.AddTransient(typeof(IPipelineBehavior<,>), behaviorType.Type);
            }
            else
            {
                foreach (var @interface in behaviorType.Interfaces)
                {
                    services.AddTransient(@interface, behaviorType.Type);
                }
            }
        }
    }
}