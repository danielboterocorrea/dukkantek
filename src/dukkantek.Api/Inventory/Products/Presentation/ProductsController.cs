using System.Net;
using dukkantek.Api.Attributes;
using dukkantek.Api.Inventory.Products.Add.Application;
using dukkantek.Api.Inventory.Products.Add.Presentation;
using dukkantek.Api.Inventory.Products.ChangeStatuses.Application;
using dukkantek.Api.Inventory.Products.ChangeStatuses.Presentation;
using dukkantek.Api.Inventory.Products.CountProducts;
using dukkantek.Api.Inventory.Products.CountProducts.Presentation;
using dukkantek.Api.Inventory.Products.Domain;
using dukkantek.Api.Shared.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dukkantek.Api.Inventory.Products.Presentation;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IProductStore _productStore;

    public ProductsController(IMediator mediator, IProductStore productStore)
    {
        _mediator = mediator;
        _productStore = productStore;
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost(Name = nameof(SellProduct))]
    public async Task<ActionResult> SellProduct([FromBody] SellProductModel request, CancellationToken cancellationToken)
    {
        var command = new SellProduct.Command(request.Name, request.Barcode, request.Description, request.CategoryName, request.Weighted);
        var result = await _mediator.Send(command, cancellationToken);
        return StatusCode((int)HttpStatusCode.Created, result.Product.ToChangeStatusResponse());
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{Id}/status",Name = nameof(ChangeStatus))]
    public async Task<ActionResult> ChangeStatus([FromRouteAndBody] ChangeStatusModel request, CancellationToken cancellationToken)
    {
        var command = new ChangeStatus.Command(request.Id, request.Body!.Status);
        var result = await _mediator.Send(command, cancellationToken);
    
        //This can be improved by taking into account all the ErrorTypes and converting them into specific HttpStatusCodes
        if (result.IsFailure)
            return StatusCode((int)HttpStatusCode.BadRequest, Error.ToError(result.Error)); 
            
        return StatusCode((int)HttpStatusCode.Created, result.Value.Product.ToChangeStatusResponse());
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("count-products",Name = nameof(CountProducts))]
    public async Task<ActionResult> CountProducts(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CountProduct.Command(), cancellationToken);
    
        return StatusCode((int)HttpStatusCode.Created, result.ToCountProductResponse());
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet(Name = nameof(GetAll))]
    public async Task<ActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok((await _productStore.GetAllAsync(cancellationToken)).Select(product => product.ToProductResponse()));
    }
}