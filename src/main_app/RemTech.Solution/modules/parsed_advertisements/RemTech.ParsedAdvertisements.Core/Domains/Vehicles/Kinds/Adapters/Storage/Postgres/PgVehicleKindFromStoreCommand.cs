using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports.Storage.Postgres;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Adapters.Storage.Postgres;

public sealed class PgVehicleKindFromStoreCommand(string text) : IPgVehicleKindFromStoreCommand
{
    private readonly string _sql = string.Intern("""
                                                 SELECT id, text FROM parsed_advertisements_module.vehicle_kinds
                                                 WHERE text = @text
                                                 """);

    public async Task<VehicleKind> Fetch(NpgsqlConnection connection, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Vehicle kind name is empty.");
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                        new PgCommand(connection, _sql))
                    .With("@text", text))).AsyncReader(ct);
        return await new PgSingleRiddenVehicleKind(reader).Read();
    }
}