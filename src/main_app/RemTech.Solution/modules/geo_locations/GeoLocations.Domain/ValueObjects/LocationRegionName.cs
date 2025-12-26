using RemTech.Core.Shared.Result;

namespace GeoLocations.Domain.ValueObjects;

public sealed record LocationRegionName
{
    public const int MaxLength = 50;
    public string Value { get; }

    private LocationRegionName(string value) => Value = value;

    public static Status<LocationRegionName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Название региона было пустым.");

        if (value.Length > MaxLength)
            return Error.Validation(
                $"Название региона не может быть больше: {MaxLength} символов."
            );

        return new LocationRegionName(value);
    }
}
