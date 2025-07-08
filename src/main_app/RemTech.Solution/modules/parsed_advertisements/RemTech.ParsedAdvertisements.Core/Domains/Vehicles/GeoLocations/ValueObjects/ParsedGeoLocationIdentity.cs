namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

public sealed record ParsedGeoLocationIdentity
{
    private readonly ParsedGeolocationId _id;
    private readonly ParsedGeolocationText _text;

    public ParsedGeoLocationIdentity(ParsedGeolocationText text)
        : this(new ParsedGeolocationId(), text) { }

    public ParsedGeoLocationIdentity(ParsedGeolocationId id, ParsedGeolocationText text)
    {
        _id = id;
        _text = text;
    }

    public ParsedGeolocationId ReadId() => _id;

    public ParsedGeolocationText ReadText() => _text;
}
