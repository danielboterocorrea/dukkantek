using CSharpFunctionalExtensions;

namespace dukkantek.Api.Inventory.Products.Domain;

public class ProductId : ValueObject
{
    public string Value { get; }

    private ProductId(string value)
        => Value = value;

    private ProductId(Guid value)
        => Value = value.ToString();

    public static ProductId NewId() => new ProductId(Guid.NewGuid().ToString());
    
    public static ProductId Parse(string id)
        => new ProductId(Guid.Parse(id));

    public static bool TryParse(string id, out ProductId productId)
    {
        var canParse = Guid.TryParse(id, out Guid result);
        productId = new ProductId(result);
        return canParse;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}