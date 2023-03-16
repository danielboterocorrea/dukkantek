using dukkantek.Api.Inventory.Products.Domain;
using dukkantek.Api.Shared.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace dukkantek.Api.Inventory.Products.ChangeStatuses.Presentation;

public record ChangeStatusModel([FromRoute] string Id, [FromBody] ChangeStatusBodyModel? Body);

public record ChangeStatusBodyModel(string? Status);

public record ChangeStatusResponse(string Id, string Name, string Barcode, string Description, string CategoryName, bool Weighted, string Status);

public static class SellProductExtensions
{
    public static ChangeStatusResponse ToChangeStatusResponse(this Product product)
        => new ChangeStatusResponse(
            product.Id.Value,
            product.Name.Value,
            product.Barcode.Value,
            product.Description.Value,
            product.CategoryName.ToString(),
            product.Weighted,
            product.Status.ToString());
}

public class ChangeStatusModelValidator : AbstractValidator<ChangeStatusModel>
{
    public ChangeStatusModelValidator()
    {
        //I need to validate more than this:
        //For instance, the Body cannot be empty or null
        
        RuleFor(changeStatusModel =>
                Product.CanChangeStatus(changeStatusModel.Body.Status))
            .Must(x => x.IsSuccess)
            .WithMessage((_, res) => Error.ToError(res.Error).ErrorCodes.First());
    }
}
