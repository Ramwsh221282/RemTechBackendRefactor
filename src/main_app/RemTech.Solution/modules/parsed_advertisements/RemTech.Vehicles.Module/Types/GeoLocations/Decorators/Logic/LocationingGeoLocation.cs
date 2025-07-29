using RemTech.Vehicles.Module.Types.Transport;

namespace RemTech.Vehicles.Module.Types.GeoLocations.Decorators.Logic;

public sealed class LocationingGeoLocation(GeoLocation origin) : GeoLocation(origin)
{
    public Vehicle Locatate(Vehicle vehicle) => new(vehicle, this);
}
