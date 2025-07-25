using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports.Storage;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Adapters.Storage.Postgres;

public sealed class PgCharacteristicFromStoreCommand(NotEmptyString text, VehicleCharacteristicValue value) : IPgCharacteristicFromStoreCommand
{
    private readonly string _sql = string.Intern("""
                                                 SELECT id, text, measuring FROM  parsed_advertisements_module.vehicle_characteristics
                                                 WHERE text = @text
                                                 """);

    public async Task<Characteristic> Fetch(NpgsqlConnection connection, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new OperationException("Название характеристики для поиска в БД было пустым.");
        await using DbDataReader reader = await new AsyncDbReaderCommand(
                new AsyncPreparedCommand(
                    new ParametrizingPgCommand(new PgCommand(connection, _sql))
                        .With("@text", (string)text)))
            .AsyncReader(ct);
        Characteristic characteristic = await new PgSingleRiddenCharacteristicFromStore(reader).Read();
        Characteristic valued = new Characteristic(characteristic, value);
        return valued;
    }
}