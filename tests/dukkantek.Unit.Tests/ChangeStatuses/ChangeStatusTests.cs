using System.Threading;
using System.Threading.Tasks;
using dukkantek.Api.Inventory.Products.Add.Application;
using dukkantek.Api.Inventory.Products.ChangeStatuses.Application;
using dukkantek.Api.Inventory.Products.Domain;
using dukkantek.Api.Shared.Domain;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace dukkantek.Unit.Tests.ChangeStatuses;

public class ChangeStatusTests : BaseFixture
{
    private IMediator _mediatr;

    public ChangeStatusTests(ServiceCollectionFixture fixture)
        : base(fixture)
    {
        _mediatr = Services.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task Product_InStock_Gets_Damaged()
    {
        var productId = await SellProductAsync();
        var command = new ChangeStatus.Command(productId.Value, ProductStatus.Damaged.ToString());
        var result = await _mediatr.Send(command);
        result.IsSuccess.Should().BeTrue();
        result.Value.Product.Status.Should().Be(ProductStatus.Damaged);
    }

    [Fact]
    public async Task Cannot_Change_Status_To_Inexistent_Product()
    {
        var command = new ChangeStatus.Command(ProductId.NewId().Value, ProductStatus.Damaged.ToString());
        var result = await _mediatr.Send(command);
        result.IsFailure.Should().BeTrue();
        var error = Error.ToError(result.Error); 
        error.ErrorType.Should().Be("item_not_found");
        error.ErrorCodes.Should().Contain("product_not_found");
    }
    
    private async Task<ProductId> SellProductAsync()
    {
        var command = new SellProduct.Command("Carrot", "123ZERT", "Green Carrot", CategoryName.Vegetable.ToString(), false);
        var result = await _mediatr.Send(command, CancellationToken.None);
        return result.Product.Id;
    }
}