using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;

namespace dukkantek.Api.Inventory.Products.Domain;

public interface IProductStore
{
    Task<ReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Product product, CancellationToken cancellationToken);
    Task<Result<Product>> GetByIdAsync(ProductId productId, CancellationToken cancellationToken);
    Task<Result<Product>> UpdateAsync(Product product, CancellationToken cancellationToken);
    
    Task<(int Damaged, int Sold, int InStock)> CountProductsAsync(CancellationToken cancellationToken);
}