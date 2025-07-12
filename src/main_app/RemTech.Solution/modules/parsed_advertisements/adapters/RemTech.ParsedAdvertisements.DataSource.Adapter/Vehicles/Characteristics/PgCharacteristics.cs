using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Characteristics.Decorators;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Characteristics;

public sealed class PgCharacteristics : IAsyncCharacteristics
{
    private readonly NpgsqlDataSource _source;

    public PgCharacteristics(NpgsqlDataSource source)
    {
        _source = source;
    }

    public async Task<Status<ICharacteristic>> Add(
        ICharacteristic characteristic,
        CancellationToken ct = default
    )
    {
        Guid id = characteristic.Identify().ReadId();
        string name = characteristic.Identify().ReadText();
        string sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.vehicle_characteristics(id, text)
            VALUES (@id, @text)
            ON CONFLICT(text)
            DO NOTHING 
            """
        );
        int affected = await new AsyncExecutedCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                    new PgCommand(await _source.OpenConnectionAsync(ct), sql)
                )
                    .With("@id", id)
                    .With("@text", name)
            )
        ).AsyncExecuted(ct);
        return affected == 0
            ? Error.Conflict($"Характеристика с названием: {name} уже присутствует.")
            : characteristic.Success();
    }

    public async Task<MaybeBag<ICharacteristic>> Find(
        CharacteristicIdentity identity,
        CancellationToken ct = default
    )
    {
        Guid id = identity.ReadId();
        string name = identity.ReadText();
        WhereFilterSqlString filter = new WhereFilterSqlString()
            .WithIf("id = @id", id, g => g != Guid.Empty)
            .WithIf("text = @text", name, n => !string.IsNullOrEmpty(n));
        if (filter.Amount() == 0)
            return new MaybeBag<ICharacteristic>();
        string sql = string.Intern(
            $"""
            SELECT id, text
            FROM parsed_advertisements_module.vehicle_characteristics
            WHERE {filter.AsSqlString()}
            """
        );
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                    new PgCommand(await _source.OpenConnectionAsync(ct), sql)
                )
                    .WithIf("@id", id, g => g != Guid.Empty)
                    .WithIf("@text", name, n => !string.IsNullOrWhiteSpace(n))
            )
        ).AsyncReader(ct);
        return await new MaybeSingleCharacteristicSqlRow(reader).Read(ct);
    }

    public void Dispose()
    {
        _source.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _source.DisposeAsync();
    }
}
