using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds.Decorators;

public sealed class TextSearchSqlSpeakingVehicleKinds(
    NpgsqlDataSource source,
    IAsyncVehicleKinds origin
) : IAsyncVehicleKinds
{
    public async Task<Status<IVehicleKind>> Add(IVehicleKind kind, CancellationToken ct = default)
    {
        MaybeBag<IVehicleKind> existing = await Find(kind.Identify(), ct);
        return existing.Any() ? existing.Take().Success() : await origin.Add(kind, ct);
    }

    public async Task<MaybeBag<IVehicleKind>> Find(
        VehicleKindIdentity identity,
        CancellationToken ct = default
    )
    {
        string name = identity.ReadText();
        if (string.IsNullOrWhiteSpace(name))
            return new MaybeBag<IVehicleKind>();
        string sql = string.Intern(
            """
            SELECT id, text FROM parsed_advertisements_module.vehicle_kinds
            WHERE document_tsvector @@ to_tsquery('russian', @name)
            ORDER BY ts_rank(document_tsvector, to_tsquery('russian', @name)) DESC
            LIMIT 1;
            """
        );
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                    new PgCommand(await source.OpenConnectionAsync(ct), sql)
                ).With("@name", new TsQuerySequenceSensitiveString(name).AsTsQueryString())
            )
        ).AsyncReader(ct);
        return !await reader.ReadAsync(ct)
            ? await origin.Find(identity, ct)
            : new VehicleKindSqlRow(reader).Read().Success();
    }

    public void Dispose() => origin.Dispose();

    public ValueTask DisposeAsync() => origin.DisposeAsync();
}
