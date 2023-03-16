using CSharpFunctionalExtensions;
using dukkantek.Api.Shared.Domain;

namespace dukkantek.Api.Inventory.Products.Domain;

public class Product : Entity<ProductId>
{
    protected Product(ProductId id, Name name, Barcode barcode, Description description, CategoryName categoryName, bool weighted, ProductStatus status)
        : base(id)
    {
        Name = name;
        Barcode = barcode;
        Description = description;
        CategoryName = categoryName;
        Weighted = weighted;
        Status = status;
    }

    public Name Name { get; }
    public Barcode Barcode { get; }
    public Description Description { get; }
    public CategoryName CategoryName { get; }
    public bool Weighted { get; }
    public ProductStatus Status { get; private set; }
    
    //Can execute pattern by Vladimir Khorikov
    //https://enterprisecraftsmanship.com/posts/validation-and-ddd/
    public static Result CanCreate(string? name, string? barcode, string? description, string? categoryName, bool? weighted, string? status)
    {
        var result = Result.Combine(new[]
        {
            Name.CanCreate(name),
            Barcode.CanCreate(barcode),
            Description.CanCreate(description)
        });
        if (result.IsFailure)
            return result;

        if(categoryName is null)
            return Result.Failure(Error.Invalid("category_name_required"));
        
        bool canParse = Enum.TryParse(categoryName, true, out CategoryName categoryNameOut);
        if (!canParse)
            return Result.Failure(Error.Invalid("category_name_invalid"));
        
        if(weighted is null)
            return Result.Failure(Error.Invalid("weighted_required"));

        var isStatusValid = ValidateStatus(status);
        if(isStatusValid.IsFailure)
            return isStatusValid;
        
        return Result.Success();
    }

    public static Product Create(string? name, string? barcode, string? description, string? categoryName, bool? weighted, string? status)
    {
        if (CanCreate(name, barcode, description, categoryName, weighted, status).IsFailure)
            throw new Exception("Use the CanCreate before calling this method");

        var categoryNameEnum = Enum.Parse<CategoryName>(categoryName, true);
        var productStatusEnum = Enum.Parse<ProductStatus>(status, true);
        return new Product(ProductId.NewId(), Name.Create(name), Barcode.Create(barcode), Description.Create(description), categoryNameEnum, weighted!.Value, productStatusEnum);
    }

    public static Result CanChangeStatus(string? status)
    {
        return ValidateStatus(status);
    }

    public Product ChangeStatus(string? status)
    {
        if(CanChangeStatus(status).IsFailure)
            throw new Exception("Use the CanChangeStatus before calling this method");
        
        var statusEnum = Enum.Parse<ProductStatus>(status, true);
        Status = statusEnum;
        return this;
    }
    
    
    private static Result ValidateStatus(string? status)
    {
        if(status is null)
            return Result.Failure(Error.Invalid("product_status_required"));
        
        bool canParseStatus = Enum.TryParse(status, true, out ProductStatus productStatusOut);
        if (!canParseStatus)
            return Result.Failure(Error.Invalid("product_status_invalid"));
        
        return Result.Success();
    }
}