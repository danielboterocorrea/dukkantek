using System;
using Microsoft.Extensions.DependencyInjection;

namespace dukkantek.Integration.Tests;

public class ServiceCollectionFixture : IDisposable
{
    public ServiceCollectionFactory ServiceCollectionFactory { get; private set; }

    public IServiceProvider ServiceProvider
        => ServiceCollectionFactory.ServiceProvider;

    public void Initialize(Action<IServiceCollection> configure)
        => ServiceCollectionFactory ??= new ServiceCollectionFactory(configure);
    
    public void Dispose()
        => ServiceCollectionFactory = null!;
}