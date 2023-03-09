using System;
using System.Threading;
using System.Threading.Tasks;
using dukkantek.Api.Inventory.Products.Domain;
using dukkantek.Api.Shared.Domain;
using dukkantek.Tests.Common;
using dukkantek.Tests.Common.InMemoryStores;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace dukkantek.Integration.Tests.Products;

public class ProductTests : BaseFixture
{
    private IProductStore _productStore;

    protected ProductTests(ServiceCollectionFixture fixture, Action<IServiceCollection> configure)
    : base(fixture, configure)
    {
        _productStore = Services.GetRequiredService<IProductStore>();
    }
    
    public ProductTests(ServiceCollectionFixture fixture)
        : this(fixture, collection => {})
    {
        
    }
    
    [Fact]
    public async Task Product_Is_Inserted_Into_Database()
    {
        var product = Product.Create("Orange", "234GH", "Orange Orange", CategoryName.Fruit.ToString(), true, ProductStatus.InStock.ToString());
        await _productStore.AddAsync(product, CancellationToken.None);
        var productDb = await _productStore.GetByIdAsync(product.Id, CancellationToken.None);
        productDb.IsSuccess.Should().BeTrue();
        productDb.Value.Should().BeEquivalentTo(product);
    }

    [Fact]
    public async Task Get_Not_Found_When_Product_Doesnt_Exist()
    {
        var productDb = await _productStore.GetByIdAsync(ProductId.NewId(), CancellationToken.None);
        productDb.IsFailure.Should().BeTrue();
        var error = Error.ToError(productDb.Error); 
        error.ErrorType.Should().Be("item_not_found");
        error.ErrorCodes.Should().Contain("product_not_found");
    }
    
    [Fact]
    public async Task Update_Product_That_Doesnt_Exists_Return_Not_Found()
    {
        var product = Product.Create("Orange", "234GH", "Orange Orange", CategoryName.Fruit.ToString(), true, ProductStatus.InStock.ToString());
        product.ChangeStatus(ProductStatus.Sold.ToString());

        var productDb = await _productStore.UpdateAsync(product, CancellationToken.None);
        productDb.IsFailure.Should().BeTrue();
        var error = Error.ToError(productDb.Error); 
        error.ErrorType.Should().Be("item_not_found");
        error.ErrorCodes.Should().Contain("product_not_found");
    }
    
    [Fact]
    public async Task Product_Is_Updated()
    {
        var product = Product.Create("Orange", "234GH", "Orange Orange", CategoryName.Fruit.ToString(), true, ProductStatus.InStock.ToString());
        await _productStore.AddAsync(product, CancellationToken.None);
        
        var productDb = await _productStore.GetByIdAsync(product.Id, CancellationToken.None);
        productDb.Value.ChangeStatus(ProductStatus.Sold.ToString());
        
        var updatedProductResult = await _productStore.UpdateAsync(productDb.Value, CancellationToken.None);
        var productUpdatedDb = await _productStore.GetByIdAsync(product.Id, CancellationToken.None);
        
        updatedProductResult.IsSuccess.Should().BeTrue();
        productUpdatedDb.Value.Status.Should().Be(ProductStatus.Sold);
    }

    [Fact]
    public async Task Count_Products_Can_Be_Calculated()
    {
        await InsertProductsAsync();
        var counters = await _productStore.CountProductsAsync(CancellationToken.None);
        counters.Damaged.Should().BeGreaterOrEqualTo(5);
        counters.Sold.Should().BeGreaterOrEqualTo(4);
        counters.InStock.Should().BeGreaterOrEqualTo(3);
    }

    private async Task InsertProductsAsync()
    {
        await _productStore.AddAsync(Product.Create("Orange1", "234GH1", "Orange Orange1", CategoryName.Fruit.ToString(), true, ProductStatus.InStock.ToString()), CancellationToken.None);
        await _productStore.AddAsync(Product.Create("Orange1", "234GH1", "Orange Orange1", CategoryName.Fruit.ToString(), true, ProductStatus.InStock.ToString()), CancellationToken.None);
        await _productStore.AddAsync(Product.Create("Orange1", "234GH1", "Orange Orange1", CategoryName.Fruit.ToString(), true, ProductStatus.InStock.ToString()), CancellationToken.None);
        
        await _productStore.AddAsync(Product.Create("Orange2", "234GH2", "Orange Orange2", CategoryName.Fruit.ToString(), true, ProductStatus.Sold.ToString()), CancellationToken.None);
        await _productStore.AddAsync(Product.Create("Orange2", "234GH2", "Orange Orange2", CategoryName.Fruit.ToString(), true, ProductStatus.Sold.ToString()), CancellationToken.None);
        await _productStore.AddAsync(Product.Create("Orange2", "234GH2", "Orange Orange2", CategoryName.Fruit.ToString(), true, ProductStatus.Sold.ToString()), CancellationToken.None);
        await _productStore.AddAsync(Product.Create("Orange2", "234GH2", "Orange Orange2", CategoryName.Fruit.ToString(), true, ProductStatus.Sold.ToString()), CancellationToken.None);
        
        await _productStore.AddAsync(Product.Create("Orange3", "234GH3", "Orange Orange3", CategoryName.Fruit.ToString(), true, ProductStatus.Damaged.ToString()), CancellationToken.None);
        await _productStore.AddAsync(Product.Create("Orange3", "234GH3", "Orange Orange3", CategoryName.Fruit.ToString(), true, ProductStatus.Damaged.ToString()), CancellationToken.None);
        await _productStore.AddAsync(Product.Create("Orange3", "234GH3", "Orange Orange3", CategoryName.Fruit.ToString(), true, ProductStatus.Damaged.ToString()), CancellationToken.None);
        await _productStore.AddAsync(Product.Create("Orange3", "234GH3", "Orange Orange3", CategoryName.Fruit.ToString(), true, ProductStatus.Damaged.ToString()), CancellationToken.None);
        await _productStore.AddAsync(Product.Create("Orange3", "234GH3", "Orange Orange3", CategoryName.Fruit.ToString(), true, ProductStatus.Damaged.ToString()), CancellationToken.None);
    }
}

//I am running my integration tests against my double tests (fakes) since they have to behave the same way.
//Check these talk out from Valentina Cupac: https://www.youtube.com/watch?v=3wxiQB2-m2k
//https://www.youtube.com/watch?v=IZWLnn2fNko
public class ProductDoubleTests : ProductTests
{
    public ProductDoubleTests(ServiceCollectionFixture fixture)
        : base(fixture, sp => sp.Override<IProductStore, InMemoryProductStore>(ServiceLifetime.Singleton))
    { }
}