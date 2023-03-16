using CSharpFunctionalExtensions;
using dukkantek.Api.Shared.Domain;

namespace dukkantek.Api.Inventory.Products.Domain;

public class Description : ValueObject
{
    public string Value { get; private init; }

    public static Result CanCreate(string? value)
    {
        if (value is null)
            return Result.Failure(Error.Invalid("description_required"));

        if(string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            return Result.Failure(Error.Invalid("description_invalid"));
        
        return Result.Success();
    }

    public static Description Create(string? value)
    {
        if (CanCreate(value).IsFailure)
            throw new Exception("Use the CanCreate before calling this method");

        return new Description { Value = value! };
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}