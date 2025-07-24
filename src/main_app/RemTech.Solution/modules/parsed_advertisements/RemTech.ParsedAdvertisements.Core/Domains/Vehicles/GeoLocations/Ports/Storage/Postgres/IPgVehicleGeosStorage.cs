namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports.Storage.Postgres;

public interface IPgVehicleGeosStorage
{
    Task<GeoLocation> Get(GeoLocation location, CancellationToken ct = default);
}