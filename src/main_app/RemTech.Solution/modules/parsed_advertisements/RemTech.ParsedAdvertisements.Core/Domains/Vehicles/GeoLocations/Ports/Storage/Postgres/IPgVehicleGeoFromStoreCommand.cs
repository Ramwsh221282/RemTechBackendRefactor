using Npgsql;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports.Storage.Postgres;

public interface IPgVehicleGeoFromStoreCommand
{
    Task<GeoLocation> Fetch(NpgsqlConnection connection, CancellationToken ct = default);
}