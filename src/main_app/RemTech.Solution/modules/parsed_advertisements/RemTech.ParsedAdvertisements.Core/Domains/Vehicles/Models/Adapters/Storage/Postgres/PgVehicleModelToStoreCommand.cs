using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Ports.Storage.Postgres;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Adapters.Storage.Postgres;

public sealed class PgVehicleModelToStoreCommand(Guid id, string name) : IPgVehicleModelToStoreCommand
{
    private readonly string _sql = string.Intern("""
                                                INSERT INTO parsed_advertisements_module.vehicle_models(id, text)
                                                VALUES (@id, @text)
                                                ON CONFLICT(text) DO NOTHING;
                                                """);

    public async Task<int> Execute(NpgsqlConnection connection, CancellationToken ct = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Vehicle model id is empty");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Vehicle model name is empty");
        return await new AsyncExecutedCommand(
                new AsyncPreparedCommand(
                    new ParametrizingPgCommand(new PgCommand(connection, _sql))
                        .With("@id", id)
                        .With("@text", name)))
            .AsyncExecuted(ct);
    }
}