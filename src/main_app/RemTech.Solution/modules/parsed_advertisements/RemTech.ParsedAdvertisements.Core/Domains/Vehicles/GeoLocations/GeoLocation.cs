using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;

public class GeoLocation : IGeoLocation
{
    protected virtual GeoLocationIdentity Identity { get; }

    public GeoLocation(GeoLocationIdentity identity) =>
        Identity = identity;

    public GeoLocation(GeoLocation origin) =>
        Identity = origin.Identity;

    public GeoLocationIdentity Identify() => Identity;
    
    public Vehicle Print(Vehicle vehicle) => new(vehicle, this);

    public PgVehicleGeoFromStoreCommand FromStoreCommand() =>
        new(Identity.ReadText());
}
