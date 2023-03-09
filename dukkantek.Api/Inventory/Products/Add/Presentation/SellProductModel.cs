using dukkantek.Api.Inventory.Products.Domain;
using dukkantek.Api.Shared.Domain;
using FluentValidation;

namespace dukkantek.Api.Inventory.Products.Add.Presentation;

public record SellProductModel(string? Name, string? Barcode, string? Description, string? CategoryName, bool? Weighted);

public record SellProductResponse(string Id, string Name, string Barcode, string Description, string CategoryName, bool Weighted, string Status);

public static class SellProductExtensions
{
    public static SellProductResponse ToProductResponse(this Product product)
        => new SellProductResponse(
            product.Id.Value,
            product.Name.Value,
            product.Barcode.Value,
            product.Description.Value,
            product.CategoryName.ToString(),
            product.Weighted,
            product.Status.ToString());
}
public class SellProductModelValidator : AbstractValidator<SellProductModel>
{
    public SellProductModelValidator()
    {
        RuleFor(productModel =>
                Product.CanCreate(productModel.Name,
                    productModel.Barcode,
                    productModel.Description,
                    productModel.CategoryName,
                    productModel.Weighted,
                    ProductStatus.InStock.ToString()))
            .Must(x => x.IsSuccess)
            .WithMessage((_, res) => Error.ToError(res.Error).ErrorCodes.First());
    }
}
 