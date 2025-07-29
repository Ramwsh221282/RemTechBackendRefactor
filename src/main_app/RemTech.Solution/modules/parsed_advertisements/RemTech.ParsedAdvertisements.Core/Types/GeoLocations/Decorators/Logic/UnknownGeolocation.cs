using RemTech.ParsedAdvertisements.Core.Types.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Types.GeoLocations.Decorators.Logic;

public sealed class UnknownGeolocation : GeoLocation
{
    public UnknownGeolocation()
        : base(new GeoLocationIdentity()) { }
}
