using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;

public sealed class GeoLocation : IGeoLocation
{
    private readonly GeoLocationIdentity _identity;

    public GeoLocation(GeoLocationIdentity identity)
    {
        _identity = identity;
    }

    public GeoLocation(GeolocationText text)
        : this(new GeoLocationIdentity(text)) { }

    public GeoLocation(GeoLocationId id, GeolocationText text)
        : this(new GeoLocationIdentity(id, text)) { }

    public GeoLocationIdentity Identify() => _identity;
}
