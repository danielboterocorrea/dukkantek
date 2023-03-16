using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace dukkantek.Tests.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Override<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime)
    {
        services.RemoveAll(typeof(TService));
        services.Add(ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), lifetime));
        return services;
    }

    public static IServiceCollection Override<TService>(this IServiceCollection services, Func<IServiceProvider, object> factory, ServiceLifetime lifetime)
    {
        services.RemoveAll(typeof(TService));
        services.Add(ServiceDescriptor.Describe(typeof(TService), factory, lifetime));
        return services;
    }
}