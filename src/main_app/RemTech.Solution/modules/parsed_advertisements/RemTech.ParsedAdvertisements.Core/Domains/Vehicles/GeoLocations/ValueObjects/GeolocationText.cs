using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

public sealed record GeolocationText
{
    private readonly NotEmptyString _text;

    public GeolocationText(NotEmptyString text) => _text = text;

    public GeolocationText(string? text)
        : this(new NotEmptyString(text)) { }

    public static implicit operator bool(GeolocationText text) => text._text;

    public static implicit operator string(GeolocationText text) => text._text;

    public static implicit operator NotEmptyString(GeolocationText text) => text._text;
}
