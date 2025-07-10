using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;

public abstract class GeoLocationEnvelope(GeoLocationIdentity identity) : IGeoLocation
{
    private readonly GeoLocationIdentity _identity = identity;

    public GeoLocationIdentity Identify() => _identity;
}
