using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports.Storage;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Adapters.Storage.Postgres;

public sealed class PgCharacteristicFromStoreCommand(NotEmptyString text) : IPgCharacteristicFromStoreCommand
{
    private readonly string _sql = string.Intern("""
                                                 SELECT id, text, measuring FROM  parsed_advertisements_module.vehicle_characteristics
                                                 WHERE text = @text
                                                 """);

    public async Task<Characteristic> Fetch(NpgsqlConnection connection, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Characteristic name is empty");
        await using DbDataReader reader = await new AsyncDbReaderCommand(
                new AsyncPreparedCommand(
                    new ParametrizingPgCommand(new PgCommand(connection, _sql))
                        .With("@text", (string)text)))
            .AsyncReader(ct);
        return await new PgSingleRiddenCharacteristicFromStore(reader).Read();
    }
}