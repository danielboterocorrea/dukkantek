using System.Collections.Generic;
using dukkantek.Api.Inventory.Products.ChangeStatuses.Presentation;
using dukkantek.Api.Inventory.Products.Domain;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace dukkantek.Unit.Tests.ChangeStatuses;

public class ChangeStatusValidationsTests
{
    private readonly ChangeStatusModelValidator _validator;
    
    public ChangeStatusValidationsTests()
        => _validator = new ChangeStatusModelValidator();
    
    public static IEnumerable<object?[]> ValidProduct
        => new List<object?[]>
        {
            new object?[] { ProductStatus.Damaged.ToString() },
            //Add more valid combinations
        };
    
    [Theory]
    [MemberData(nameof(ValidProduct))]
    public void Change_To_Valid_Status(string status)
    {
        var model = new ChangeStatusModel(ProductId.NewId().Value, new ChangeStatusBodyModel(status));

        var result = _validator.TestValidate(model);
        result.IsValid.Should().BeTrue();
    }
    
    public static IEnumerable<object?[]> InvalidProduct
        => new List<object?[]>
        {
            new object?[] { null, "product_status_required" },
            new object?[] { "", "product_status_invalid" },
            new object?[] { " ", "product_status_invalid" },
            //Add more invalid combinations
        };
    
    [Theory]
    [MemberData(nameof(InvalidProduct))]
    public void Product_Is_Invalid(string? status, string expectedError)
    {
        var model = new ChangeStatusModel(ProductId.NewId().Value, new ChangeStatusBodyModel(status));

        var result = _validator.TestValidate(model);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage.Equals(expectedError));
    }
    
}