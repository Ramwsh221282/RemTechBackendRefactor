using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports.Storage;

public interface IPgVehicleBrandStoreCommand
{
    Task<int> Execute(NpgsqlConnection connection, CancellationToken ct = default);
}