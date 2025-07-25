using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports.Storage.Postgres;

public interface IPgVehicleGeoToStoreCommand
{
    Task<int> Execute(NpgsqlConnection connection, CancellationToken ct = default);
}