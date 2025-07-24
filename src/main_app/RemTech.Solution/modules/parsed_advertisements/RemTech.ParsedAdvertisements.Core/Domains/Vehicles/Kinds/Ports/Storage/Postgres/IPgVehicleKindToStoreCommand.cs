using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports.Storage.Postgres;

public interface IPgVehicleKindToStoreCommand
{
    Task<int> Execute(NpgsqlConnection connection, CancellationToken ct = default);
}