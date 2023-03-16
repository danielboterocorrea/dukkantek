using CSharpFunctionalExtensions;

namespace dukkantek.Api.Shared.Domain;

public class Error : ValueObject
{
    public string ErrorType { get; private init; }
    public string[] ErrorCodes { get; private init; }

    public static Error Invalid(params string[] errorCodes)
        => new Error { ErrorType = "invalid_request", ErrorCodes = errorCodes };

    public static Error NotFound(params string[] errorCodes)
        => new Error { ErrorType = "item_not_found", ErrorCodes = errorCodes };

    public static Error ServiceError(params string[] errorCodes)
        => new Error { ErrorType = "service_error", ErrorCodes = errorCodes };
    
    public static implicit operator string(Error e)
        => $"{e.ErrorType}@{string.Join(",", e.ErrorCodes)}";

    public static Error ToError(string error)
    {
        var split = error.Split("@");
        return new Error
        {
            ErrorType = split[0].Trim(),
            ErrorCodes = split[1].Split(",").Select(x => x.Trim()).ToArray()
        };
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ErrorType;
        yield return ErrorCodes;
    }
}