using System.Threading;
using System.Threading.Tasks;
using dukkantek.Api.Inventory.Products.Add.Application;
using dukkantek.Api.Inventory.Products.ChangeStatuses.Application;
using dukkantek.Api.Inventory.Products.CountProducts;
using dukkantek.Api.Inventory.Products.Domain;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace dukkantek.Unit.Tests.CountProducts;

public class CountProductsTests : BaseFixture
{
    private readonly IMediator _mediatR;

    public CountProductsTests(ServiceCollectionFixture fixture)
        : base(fixture)
    {
        _mediatR = Services.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task Counts_Of_Products_Is_Calculated()
    {
        await InitialiseProductsAsync();
        var command = new CountProduct.Command();
        var result = await _mediatR.Send(command, CancellationToken.None);
        result.NumberOfProductsDamaged.Should().BeGreaterOrEqualTo(4);
        result.NumberOfProductsSold.Should().BeGreaterOrEqualTo(5);
        result.NumberOfProductsInStock.Should().BeGreaterOrEqualTo(3);
    }

    private async Task InitialiseProductsAsync()
    {
        await AddInStockProductsAsync();
        await AddInStockProductsAsync();
        await AddInStockProductsAsync();

        await AddDamagedProductsAsync();
        await AddDamagedProductsAsync();
        await AddDamagedProductsAsync();
        await AddDamagedProductsAsync();

        await AddSoldProductsAsync();
        await AddSoldProductsAsync();
        await AddSoldProductsAsync();
        await AddSoldProductsAsync();
        await AddSoldProductsAsync();
    }

    private async Task AddInStockProductsAsync()
    {
        var sellCommand = new SellProduct.Command("Onion", "12345ABC", "White Onion", CategoryName.Vegetable.ToString(), true);
        await _mediatR.Send(sellCommand, CancellationToken.None);
    }
    
    private async Task AddDamagedProductsAsync()
    {
        var sellCommand = new SellProduct.Command("Onion", "12345ABC", "White Onion", CategoryName.Vegetable.ToString(), true);
        var reponse = await _mediatR.Send(sellCommand, CancellationToken.None);
        await _mediatR.Send(new ChangeStatus.Command(reponse.Product.Id.Value, ProductStatus.Damaged.ToString()));
    }
    
    private async Task AddSoldProductsAsync()
    {
        var sellCommand = new SellProduct.Command("Onion", "12345ABC", "White Onion", CategoryName.Vegetable.ToString(), true);
        var reponse = await _mediatR.Send(sellCommand, CancellationToken.None);
        await _mediatR.Send(new ChangeStatus.Command(reponse.Product.Id.Value, ProductStatus.Sold.ToString()));
    }
}