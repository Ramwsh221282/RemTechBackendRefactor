using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds.Decorators;

public sealed class TsQueryPgVehicleKinds(NpgsqlDataSource source, IAsyncVehicleKinds origin)
    : IAsyncVehicleKinds
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
            WITH input_words AS (
                SELECT
                    unnest(string_to_array(
                            lower(regexp_replace(@input, '[^a-zA-Z0-9А-Яа-я ]', '', 'g')),
                            ' '
                           )) AS input_word
            )
            SELECT
                COALESCE(e.text, 'Не найдено') AS text,
                COALESCE(max_sim.rank, 0) AS rank,
                e.id AS id
            FROM input_words iw
                     LEFT JOIN LATERAL (
                SELECT
                    id,
                    text,
                    ts_rank(vehicle_kinds.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                FROM parsed_advertisements_module.vehicle_kinds
                WHERE vehicle_kinds.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                ORDER BY rank DESC
                LIMIT 1
                ) e ON true
                     LEFT JOIN LATERAL (
                SELECT ts_rank(vehicle_kinds.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                FROM parsed_advertisements_module.vehicle_kinds
                WHERE vehicle_kinds.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                ORDER BY rank DESC
                LIMIT 1
                ) max_sim ON true
            WHERE iw.input_word != '' AND max_sim.rank > 0
            ORDER BY rank DESC
            LIMIT 1;
            """
        );
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                    new PgCommand(await source.OpenConnectionAsync(ct), sql)
                ).With("@input", name)
            )
        ).AsyncReader(ct);
        return await new MaybeSingleVehicleKindSqlRow(reader).Read(ct);
    }

    public void Dispose() => origin.Dispose();

    public ValueTask DisposeAsync() => origin.DisposeAsync();
}
