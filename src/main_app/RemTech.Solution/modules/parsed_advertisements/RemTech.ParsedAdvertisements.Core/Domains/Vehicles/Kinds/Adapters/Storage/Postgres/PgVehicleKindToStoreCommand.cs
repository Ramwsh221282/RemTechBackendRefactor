using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports.Storage.Postgres;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Adapters.Storage.Postgres;

public sealed class PgVehicleKindToStoreCommand(string text, Guid id) : IPgVehicleKindToStoreCommand
{
    private readonly string _sql = string.Intern("""
                                                INSERT INTO parsed_advertisements_module.vehicle_kinds(id, text)
                                                VALUES (@id, @text)
                                                ON CONFLICT(text) DO NOTHING;
                                                """);

    public async Task<int> Execute(NpgsqlConnection connection, CancellationToken ct = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Vehicle kind id is empty");
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Vehicle kind name is empty.");
        return await new AsyncExecutedCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                        new PgCommand(connection, _sql))
                    .With("@id", id)
                    .With("@text", text))).AsyncExecuted(ct);
    }
}