using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports.Storage;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Adapters.Storage.Postgres;

public sealed class PgCharacteristicToStoreCommand(Guid id, string text, string measuring)
    : IPgCharacteristicToStoreCommand
{
    private readonly string _sql = string.Intern("""
                                                INSERT INTO parsed_advertisements_module.vehicle_characteristics(id, text, measuring)
                                                VALUES (@id, @text, @measuring)
                                                ON CONFLICT(text) DO NOTHING;
                                                """);

    public async Task<int> Execute(NpgsqlConnection connection, CancellationToken ct = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Characteristic id is empty");
        if (string.IsNullOrWhiteSpace(measuring))
            throw new ArgumentException("Characteristic measure is empty");
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Characteristic name is empty");
        return await new AsyncExecutedCommand
            (new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, _sql))
                    .With("@id", id)
                    .With("@text", text)
                    .With("@measuring", measuring)))
            .AsyncExecuted(ct);
    }
}