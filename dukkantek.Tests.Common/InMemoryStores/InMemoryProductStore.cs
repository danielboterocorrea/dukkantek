using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using dukkantek.Api.Inventory.Products.Domain;
using dukkantek.Api.Shared.Domain;

namespace dukkantek.Tests.Common.InMemoryStores;

public class InMemoryProductStore : IProductStore
{
    private readonly ConcurrentDictionary<ProductId, Product> _data = new();
    
    public Task AddAsync(Product product, CancellationToken cancellationToken)
        => Task.FromResult(_data.AddOrUpdate(product.Id, product, (_, _) => product));

    public Task<Result<Product>> GetByIdAsync(ProductId productId, CancellationToken cancellationToken)
    {
        var exists = _data.TryGetValue(productId, out Product productDb);
        if (!exists)
            return Task.FromResult(Result.Failure<Product>(Error.NotFound("product_not_found")));

        return Task.FromResult(Result.Success(productDb));
    }

    public Task<Result<Product>> UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        var exists = _data.TryGetValue(product.Id, out Product productDb);
        if (!exists)
            return Task.FromResult(Result.Failure<Product>(Error.NotFound("product_not_found")));

        _data.AddOrUpdate(product.Id, product, (_, _) => product);
        
        return Task.FromResult(Result.Success(productDb));
    }

    public Task<(int Damaged, int Sold, int InStock)> CountProductsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult((
            _data.Values.Count(v => v.Status == ProductStatus.Damaged),
            _data.Values.Count(v => v.Status == ProductStatus.Sold),
            _data.Values.Count(v => v.Status == ProductStatus.InStock)));
    }

    public Task<ReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_data.Values.ToList().AsReadOnly());
    }
}