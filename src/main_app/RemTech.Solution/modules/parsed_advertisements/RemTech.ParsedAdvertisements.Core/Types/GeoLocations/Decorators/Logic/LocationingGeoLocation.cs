using RemTech.ParsedAdvertisements.Core.Types.Transport;

namespace RemTech.ParsedAdvertisements.Core.Types.GeoLocations.Decorators.Logic;

public sealed class LocationingGeoLocation(GeoLocation origin) : GeoLocation(origin)
{
    public Vehicle Locatate(Vehicle vehicle) => new(vehicle, this);
}
