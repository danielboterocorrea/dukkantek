using System.Collections.Generic;
using dukkantek.Api.Inventory.Products.Add.Presentation;
using dukkantek.Api.Inventory.Products.Domain;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace dukkantek.Unit.Tests.Products;

public class ProductValidationsTests
{
    private readonly SellProductModelValidator _validator;
    
    public ProductValidationsTests()
        => _validator = new SellProductModelValidator();
    
    public static IEnumerable<object?[]> ValidProduct
        => new List<object?[]>
        {
            new object?[] { "Apple", "AZE234", "Black Apple", CategoryName.Fruit.ToString(), true },
            //Add more valid combinations
        };
    
    [Theory]
    [MemberData(nameof(ValidProduct))]
    public void Product_Is_Valid(string? name, string? barcode, string? description, string? categoryName, bool? weighted)
    {
        var model = new SellProductModel(name, barcode, description, categoryName, weighted);

        var result = _validator.TestValidate(model);
        result.IsValid.Should().BeTrue();
    }
    
    public static IEnumerable<object?[]> InvalidProduct
        => new List<object?[]>
        {
            new object?[] { null, "AZE234", "Black Apple", CategoryName.Fruit.ToString(), true },
            //Add more invalid combinations
        };
    
    [Theory]
    [MemberData(nameof(InvalidProduct))]
    public void Product_Is_Invalid(string? name, string? barcode, string? description, string? categoryName, bool? weighted)
    {
        var model = new SellProductModel(name, barcode, description, categoryName, weighted);

        var result = _validator.TestValidate(model);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage.Equals("name_required"));
    }
    
}