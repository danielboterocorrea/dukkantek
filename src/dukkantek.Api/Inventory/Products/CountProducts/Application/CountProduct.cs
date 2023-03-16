using CSharpFunctionalExtensions;
using dukkantek.Api.Inventory.Products.Domain;
using MediatR;

namespace dukkantek.Api.Inventory.Products.CountProducts;

public class CountProduct
{
    public record Command() : IRequest<Response>;
    public record Response(int NumberOfProductsSold, int NumberOfProductsDamaged, int NumberOfProductsInStock);
    
    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly IProductStore _productStore;

        public Handler(IProductStore productStore)
        {
            _productStore = productStore;
        }
        
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var counters = await _productStore.CountProductsAsync(cancellationToken);
            
            return new Response(counters.Sold, counters.Damaged, counters.InStock);
        }
    }
}