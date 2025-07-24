using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;

public abstract class GeoLocation : IGeoLocation
{
    private readonly GeoLocationIdentity _identity;

    public GeoLocation(GeoLocationIdentity identity) =>
        _identity = identity;

    public GeoLocationIdentity Identify() => _identity;
    
    public Vehicle Print(Vehicle vehicle) => new(vehicle, this);

    public PgVehicleGeoFromStoreCommand FromStoreCommand() =>
        new(_identity.ReadText());
}
