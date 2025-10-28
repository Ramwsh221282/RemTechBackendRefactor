using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.BrandContext.ValueObjects;

public sealed record BrandName
{
    public const int Length = 100;
    public string Name { get; }

    private BrandName(string name)
    {
        Name = name;
    }

    public static Status<BrandName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("Название бренда техники было пустым.");

        if (name.Length > Length)
            return Error.Validation($"Название бренда превышает длину: {Length} символов.");

        return new BrandName(name);
    }
}
