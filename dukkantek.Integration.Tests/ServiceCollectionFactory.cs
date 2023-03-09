using System;
using dukkantek.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dukkantek.Integration.Tests;

public class ServiceCollectionFactory
{
    public IServiceProvider ServiceProvider { get; }

    public ServiceCollectionFactory(Action<IServiceCollection> configureServiceCollection)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        IConfigurationBuilder configBuilder = new ConfigurationBuilder();

        var configuration = configBuilder.BuildConfiguration();
        
        var services = new ServiceCollection()
            .BuildServices(configuration)
            .AddOrReconfigureServices(configureServiceCollection);
 
        ServiceProvider = services.BuildServiceProvider();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrReconfigureServices(this IServiceCollection serviceCollection, Action<IServiceCollection> configure)
    {
        configure(serviceCollection);
        return serviceCollection;
    }
}