using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators.Logic;

public sealed class LocationingGeoLocation(GeoLocation origin) : GeoLocation(origin)
{
    public Vehicle Locatate(Vehicle vehicle) => new(vehicle, this);
}