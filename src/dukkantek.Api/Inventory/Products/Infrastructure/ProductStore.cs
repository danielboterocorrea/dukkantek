using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;
using dukkantek.Api.Infrastructure;
using dukkantek.Api.Inventory.Products.Domain;
using dukkantek.Api.Models;
using dukkantek.Api.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace dukkantek.Api.Inventory.Products.Infrastructure;

//This class should be implemented with a real implementation
//for the sake of time, I use an InMemory implementation of it but
//it should be fairly easy to put some code to access a real database
public class ProductStore : IProductStore
{
    private readonly InventoryContext _context;

    public ProductStore(InventoryContext context)
    {
        _context = context;
    }
    
    public Task<ReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_context.Products.Select(p => new ProductsProxy(p) as Product).ToList().AsReadOnly());
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await _context.AddAsync(product.ToDbModel(), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<Product>> GetByIdAsync(ProductId productId, CancellationToken cancellationToken)
    {
        var productDb = await _context.Products.FindAsync(productId.Value, cancellationToken);
        return productDb is null 
            ? Result.Failure<Product>(Error.NotFound("product_not_found")) 
            : new ProductsProxy(productDb);
    }

    public async Task<Result<Product>> UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        var productDb = await _context.Products.FindAsync(product.Id.Value, cancellationToken);
        if(productDb is null) 
            return Result.Failure<Product>(Error.NotFound("product_not_found"));

        productDb.UpdateDbModel(product);
        _context.Update(productDb);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public Task<(int Damaged, int Sold, int InStock)> CountProductsAsync(CancellationToken cancellationToken)
    {
        var counters = (from t in _context.Products
            group t by t != null
            into g
            select new
            {
                Damaged = g.Count(p => p.Status == ProductStatus.Damaged.ToString()),
                Sold = g.Count(p => p.Status == ProductStatus.Sold.ToString()),
                InStock = g.Count(p => p.Status == ProductStatus.InStock.ToString()),
            }).First();

        return Task.FromResult((counters.Damaged, counters.Sold, counters.InStock));
    }
}

public class ProductsProxy : Product
{
    public ProductsProxy(ProductDbModel productDbModel) : 
        base(
                ProductId.Parse(productDbModel.Id),
                Domain.Name.Create(productDbModel.Name),
                Barcode.Create(productDbModel.Barcode),
                Description.Create(productDbModel.Description),
                Enum.Parse<CategoryName>(productDbModel.CategoryName),
                productDbModel.Weighted,
                Enum.Parse<ProductStatus>(productDbModel.Status)
            )
    {
    }
}

public static class ProductExtensions
{
    public static ProductDbModel ToDbModel(this Product product) =>
        new ProductDbModel
        {
            Id = product.Id.Value,
            Barcode = product.Barcode.Value,
            Weighted = product.Weighted,
            Description = product.Description.Value,
            Name = product.Name.Value,
            Status = product.Status.ToString(),
            CategoryName = product.CategoryName.ToString()
        };

    public static void UpdateDbModel(this ProductDbModel productDbModel, Product product)
    {
        productDbModel.Id = product.Id.Value;
        productDbModel.Barcode = product.Barcode.Value;
        productDbModel.Weighted = product.Weighted;
        productDbModel.Description = product.Description.Value;
        productDbModel.Name = product.Name.Value;
        productDbModel.Status = product.Status.ToString();
        productDbModel.CategoryName = product.CategoryName.ToString();
    }

}