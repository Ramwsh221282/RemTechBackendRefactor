namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

public sealed record GeoLocationIdentity
{
    private readonly GeoLocationId _id;
    private readonly GeolocationText _text;
    private readonly GeolocationText _kind;

    public GeoLocationIdentity(GeolocationText text, GeolocationText kind)
        : this(new GeoLocationId(), text, kind) { }

    public GeoLocationIdentity(GeoLocationId id, GeolocationText text, GeolocationText kind)
    {
        _id = id;
        _text = text;
        _kind = kind;
    }

    public GeoLocationId ReadId() => _id;

    public GeolocationText ReadText() => _text;

    public static implicit operator bool(GeoLocationIdentity? identity)
    {
        return identity != null && identity._text && identity._id && identity._kind;
    }
}
