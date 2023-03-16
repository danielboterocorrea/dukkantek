using dukkantek.Api.Shared.Domain;
using FluentAssertions;
using Xunit;

namespace dukkantek.Unit.Tests.Shared;

public class ErrorTests
{
    [Fact]
    public void Error_String_Can_Be_Converted_To_Error_Type()
    {
        string error = Error.Invalid("something_required", "something_invalid");
        Error errorTyped = Error.ToError(error);
        errorTyped.ErrorType.Should().Be("invalid_request");
        errorTyped.ErrorCodes.Should().Contain("something_required");
        errorTyped.ErrorCodes.Should().Contain("something_invalid");
    }
}