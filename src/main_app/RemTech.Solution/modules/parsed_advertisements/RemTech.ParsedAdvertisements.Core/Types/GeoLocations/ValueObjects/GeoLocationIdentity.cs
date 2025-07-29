using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Types.GeoLocations.ValueObjects;

public sealed record GeoLocationIdentity
{
    private readonly GeoLocationId _id;
    private readonly GeolocationText _text;
    private readonly GeolocationText _kind;

    public GeoLocationIdentity()
    {
        _id = new GeoLocationId(new NotEmptyGuid(Guid.Empty));
        _text = new GeolocationText(new NotEmptyGuid(Guid.Empty));
        _kind = new GeolocationText(new NotEmptyGuid(Guid.Empty));
    }

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

    public string Kind() => _kind;

    public static implicit operator bool(GeoLocationIdentity? identity)
    {
        return identity != null && identity._text && identity._id && identity._kind;
    }
}
