namespace dukkantek.Api.Inventory.Products.CountProducts.Presentation;

public record CountProductResponse(int NumberOfProductsSold, int NumberOfProductsDamaged, int NumberOfProductsInStock);

public static class CountProductsExtensions
{
    public static CountProductResponse ToCountProductResponse(this CountProduct.Response counters)
        => new CountProductResponse(counters.NumberOfProductsSold, counters.NumberOfProductsDamaged, counters.NumberOfProductsInStock);
}