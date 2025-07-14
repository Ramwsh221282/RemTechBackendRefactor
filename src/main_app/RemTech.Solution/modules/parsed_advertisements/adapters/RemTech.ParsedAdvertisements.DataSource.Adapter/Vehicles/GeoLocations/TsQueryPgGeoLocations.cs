using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.GeoLocations;

public sealed class TsQueryPgGeoLocations(NpgsqlDataSource source, IAsyncGeoLocations locations)
    : IAsyncGeoLocations
{
    public void Dispose() => locations.Dispose();

    public ValueTask DisposeAsync() => locations.DisposeAsync();

    public async Task<Status<IGeoLocation>> Add(
        IGeoLocation location,
        CancellationToken ct = default
    )
    {
        MaybeBag<IGeoLocation> existing = await Find(location.Identify(), ct);
        return existing.Any() ? existing.Take().Success() : await locations.Add(location, ct);
    }

    public async Task<MaybeBag<IGeoLocation>> Find(
        GeoLocationIdentity identity,
        CancellationToken ct = default
    )
    {
        string text = identity.ReadText();
        if (string.IsNullOrEmpty(text))
            return await locations.Find(identity, ct);
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
                e.id AS id,
                e.kind AS kind
            FROM input_words iw
                     LEFT JOIN LATERAL (
                SELECT
                    id,
                    text,
                    kind,
                    ts_rank(geos.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                FROM parsed_advertisements_module.geos
                WHERE geos.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                ORDER BY rank DESC
                LIMIT 1
                ) e ON true
                     LEFT JOIN LATERAL (
                SELECT ts_rank(geos.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                FROM parsed_advertisements_module.geos
                WHERE geos.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
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
                ).With("@input", text)
            )
        ).AsyncReader(ct);
        return await new MaybeSingleGeoLocationSqlRow(reader).Read(ct);
    }
}
