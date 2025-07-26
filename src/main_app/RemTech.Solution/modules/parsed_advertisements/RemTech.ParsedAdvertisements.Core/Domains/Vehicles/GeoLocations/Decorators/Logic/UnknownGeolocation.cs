using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators.Logic;

public sealed class UnknownGeolocation : GeoLocation
{
    public UnknownGeolocation() : base(new GeoLocationIdentity())
    {
    }
}