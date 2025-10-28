using RemTech.Core.Shared.Result;

namespace GeoLocations.Domain.ValueObjects;

public sealed record LocationRegionKind
{
    public const int MaxLength = 20;
    public string Value { get; }

    private LocationRegionKind(string value) => Value = value;

    public static Status<LocationRegionKind> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Тип региона было пустым.");

        if (value.Length > MaxLength)
            return Error.Validation($"Тип региона не может быть больше: {MaxLength} символов.");

        return new LocationRegionKind(value);
    }
}
