using RemTech.Core.Shared.Result;

namespace GeoLocations.Domain.ValueObjects;

public sealed record CityName
{
    public const int MaxLength = 50;
    public string Value { get; }

    private CityName(string value) => Value = value;

    public static Status<CityName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Название города было пустым.");

        if (value.Length > MaxLength)
            return Error.Validation($"Название города превышает длину {MaxLength} символов.");

        return new CityName(value);
    }
}
