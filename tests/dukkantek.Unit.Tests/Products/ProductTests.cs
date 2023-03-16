using System.Threading.Tasks;
using dukkantek.Api.Inventory.Products.Add.Application;
using dukkantek.Api.Inventory.Products.Domain;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace dukkantek.Unit.Tests.Products;

public class ProductTests : BaseFixture
{
    private readonly IMediator _mediatr;

    public ProductTests(ServiceCollectionFixture fixture)
        : base(fixture)
    {
        _mediatr = Services.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task Can_Sell_Product()
    {
        var sellCommand = new SellProduct.Command("Onion", "12345ABC", "White Onion", CategoryName.Vegetable.ToString(), true);
        var result = await _mediatr.Send(sellCommand);
        result.Product.Name.Value.Should().Be("Onion");
    }
}