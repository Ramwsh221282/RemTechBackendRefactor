using RemTech.Result.Pattern;
using Vehicles.Domain.BrandContext.Errors;

namespace Vehicles.Domain.BrandContext.ValueObjects;

public sealed record BrandName
{
    public const int MaxLength = 80;
    public string Name { get; }

    private BrandName(string name) => Name = name;

    public static Result<BrandName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new BrandNameEmptyError();
        if (name.Length > MaxLength)
            return new BrandNameExceesLengthError(MaxLength);
        return new BrandName(name);
    }
}
