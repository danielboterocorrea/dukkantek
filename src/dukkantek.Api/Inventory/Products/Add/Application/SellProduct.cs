using dukkantek.Api.Inventory.Products.Domain;
using MediatR;

namespace dukkantek.Api.Inventory.Products.Add.Application;

public class SellProduct
{
    //For logging purposes, we can inject a context enricher so all these attributes are logged in the info request log.
    public record Command(string? Name, string? Barcode, string? Description, string? CategoryName, bool? Weighted) : IRequest<Response>;
    
    public record Response(Product Product);

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly IProductStore _productStore;

        public Handler(IProductStore productStore)
        {
            _productStore = productStore;
        }
        
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var product = Product.Create(
                request.Name,
                request.Barcode,
                request.Description,
                request.CategoryName,
                request.Weighted,
                ProductStatus.InStock.ToString());

            await _productStore.AddAsync(product, cancellationToken);
            
            return new Response(product);
        }
    }
}