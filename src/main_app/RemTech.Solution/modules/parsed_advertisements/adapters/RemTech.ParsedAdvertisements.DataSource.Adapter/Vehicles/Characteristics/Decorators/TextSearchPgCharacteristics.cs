using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Characteristics.Decorators;

public sealed class TextSearchPgCharacteristics(
    NpgsqlDataSource source,
    IAsyncCharacteristics origin
) : IAsyncCharacteristics
{
    public void Dispose()
    {
        origin.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return origin.DisposeAsync();
    }

    public async Task<Status<ICharacteristic>> Add(
        ICharacteristic characteristic,
        CancellationToken ct = default
    )
    {
        MaybeBag<ICharacteristic> existing = await Find(characteristic.Identify(), ct);
        return existing.Any() ? existing.Take().Success() : await origin.Add(characteristic, ct);
    }

    public async Task<MaybeBag<ICharacteristic>> Find(
        CharacteristicIdentity identity,
        CancellationToken ct = default
    )
    {
        string text = identity.ReadText();
        if (string.IsNullOrEmpty(text))
            return new MaybeBag<ICharacteristic>();
        string sql = string.Intern(
            """
            SELECT id, text, word_similarity(text, @input) as sml
            FROM parsed_advertisements_module.vehicle_characteristics
            WHERE word_similarity(text, @input) > 0.5
            ORDER BY sml DESC
            LIMIT 1;
            """
        );
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                    new PgCommand(await source.OpenConnectionAsync(ct), sql)
                ).With("@input", text)
            )
        ).AsyncReader(ct);
        MaybeBag<ICharacteristic> characteristic = await new MaybeSingleCharacteristicSqlRow(
            reader
        ).Read(ct);
        return characteristic.Any()
            ? characteristic.Take().Success()
            : await origin.Find(identity, ct);
    }
}
