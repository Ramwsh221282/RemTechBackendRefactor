using RemTech.Result.Pattern;
using Vehicles.Domain.CategoryContext.Errors;

namespace Vehicles.Domain.CategoryContext.ValueObjects;

public sealed record CategoryName
{
    public const int MaxLength = 80;
    public string Value { get; }

    private CategoryName(string value) => Value = value;

    public static Result<CategoryName> Create(string value)
    {
        if (string.IsNullOrEmpty(value))
            return new CategoryNameEmptyError();
        if (value.Length > MaxLength)
            return new CategoryNameExceesLengthError(MaxLength);
        return new CategoryName(value);
    }
}
