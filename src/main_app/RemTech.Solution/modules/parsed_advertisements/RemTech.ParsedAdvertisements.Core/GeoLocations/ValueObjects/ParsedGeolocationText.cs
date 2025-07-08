using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.GeoLocations.ValueObjects;

public sealed record ParsedGeolocationText
{
    private readonly NotEmptyString _text;

    public ParsedGeolocationText(NotEmptyString text) => _text = text;

    public static implicit operator bool(ParsedGeolocationText text) => text._text;

    public static implicit operator string(ParsedGeolocationText text) => text._text;

    public static implicit operator NotEmptyString(ParsedGeolocationText text) => text._text;
}
