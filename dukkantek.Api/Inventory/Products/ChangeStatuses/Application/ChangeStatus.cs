using CSharpFunctionalExtensions;
using dukkantek.Api.Inventory.Products.Domain;
using MediatR;

namespace dukkantek.Api.Inventory.Products.ChangeStatuses.Application;

public class ChangeStatus
{
    public record Command(string ProductId, string Status) : IRequest<Result<Response>>;
    public record Response(Product Product);
    
    public class Handler : IRequestHandler<Command, Result<Response>>
    {
        private readonly IProductStore _productStore;

        public Handler(IProductStore productStore)
        {
            _productStore = productStore;
        }
        
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var productResult = await _productStore.GetByIdAsync(ProductId.Parse(request.ProductId), cancellationToken);
            if (productResult.IsFailure)
                return Result.Failure<Response>(productResult.Error);

            productResult.Value.ChangeStatus(request.Status);
            
            await _productStore.UpdateAsync(productResult.Value, cancellationToken);
            
            return new Response(productResult.Value);
        }
    }
}