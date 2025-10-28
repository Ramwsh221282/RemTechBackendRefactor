using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.RegionContext.ValueObjects;

public sealed record RegionName
{
    public const int MaxLength = 100;
    public string Value { get; }

    private RegionName(string value) => Value = value;

    public static Status<RegionName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Название региона не может быть пустым.");

        if (value.Length > MaxLength)
            return Error.Validation($"Название региона превышает длину: {MaxLength} символов.");

        return new RegionName(value);
    }
}