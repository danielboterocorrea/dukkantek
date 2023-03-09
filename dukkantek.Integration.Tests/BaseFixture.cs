using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace dukkantek.Integration.Tests;

public class BaseFixture : IClassFixture<ServiceCollectionFixture>, IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly IServiceProvider Services;
    
    protected BaseFixture(ServiceCollectionFixture fixture)
        : this(fixture,  _ => {})

    { }
    
    protected BaseFixture(ServiceCollectionFixture fixture, Action<IServiceCollection> configure)
    {
        fixture.Initialize(configure);
        _scope = fixture.ServiceProvider.CreateScope();
        Services = _scope.ServiceProvider;
    }

    public void Dispose()
        => _scope.Dispose();
}