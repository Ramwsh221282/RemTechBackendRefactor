using RemTech.Vehicles.Module.Types.GeoLocations.ValueObjects;

namespace RemTech.Vehicles.Module.Types.GeoLocations.Decorators.Logic;

public sealed class UnknownGeolocation : GeoLocation
{
    public UnknownGeolocation()
        : base(new GeoLocationIdentity()) { }
}
