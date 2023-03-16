using System.Reflection;
using dukkantek.Api.Attributes;
using dukkantek.Api.Infrastructure;
using dukkantek.Api.Inventory.Products.Domain;
using dukkantek.Api.Inventory.Products.Infrastructure;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dukkantek.Api;

public static class ConfigureApi
{
    private static string GetEnvironment()
        => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
    
    public static IConfiguration BuildConfiguration(this IConfigurationBuilder configurationBuilder)
    {
        var configBuilder = configurationBuilder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{GetEnvironment()}.json", true);
        
        return configBuilder.Build();
    }
    
    public static IServiceCollection BuildServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection
            .AddMediatR(typeof(Program))
            .AddScoped<IProductStore, ProductStore>()
            .AddDbContextPool<InventoryContext>(options => options
                    .UseSqlServer(configuration.GetConnectionString("SqlServer"))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking))
            .AddFluentValidation(options =>
            {
                options.ImplicitlyValidateChildProperties = true;
                options.AutomaticValidationEnabled = true;

                options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            })
            .AddSwaggerGen()
            .AddEndpointsApiExplorer()
            .AddControllers(options => options.Filters.Add(typeof(ValidateModelStateAttribute)));
        
        serviceCollection.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        return serviceCollection;
    }
}