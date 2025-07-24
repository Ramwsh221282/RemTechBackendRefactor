using System.Data.Common;
using Npgsql;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace Parsing.Vehicles.DbSearch.VehicleGeos;

public sealed class TsQueryRegionCitySearch : IVehicleGeoDbSearch
{
    private readonly IVehicleGeoDbSearch _regionSearch;
    private readonly PgConnectionSource _pgConnectionSource;
    private readonly string _sql = 
        string.Intern(
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
                    c.id,
                    c.text,
                    ts_rank(c.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                FROM parsed_advertisements_module.cities c
                INNER JOIN parsed_advertisements_module.geos g ON c.region_id = g.id
                WHERE c.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                AND g.text = @region
                ORDER BY rank DESC
                LIMIT 1
                ) e ON true
                     LEFT JOIN LATERAL (
                SELECT ts_rank(c.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                FROM parsed_advertisements_module.cities c
                WHERE c.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                ORDER BY rank DESC
                LIMIT 1
                ) max_sim ON true
            WHERE iw.input_word != '' AND max_sim.rank > 0
            ORDER BY rank DESC
            LIMIT 1;                                     
            """);

    public TsQueryRegionCitySearch(PgConnectionSource pgConnectionSource, IVehicleGeoDbSearch regionSearch)
    {
        _regionSearch = regionSearch;
        _pgConnectionSource = pgConnectionSource;
    }
    
    public async Task<ParsedVehicleGeo> Search(string text)
    {
        ParsedVehicleGeo withRegion = await _regionSearch.Search(text);
        if (!withRegion)
            return withRegion;
        await using NpgsqlConnection connection = await _pgConnectionSource.Connect();
        await using DbDataReader reader = await new AsyncDbReaderCommand(
                new AsyncPreparedCommand(
                    new ParametrizingPgCommand(
                            new PgCommand(connection, _sql))
                        .With("@input", text)
                        .With("@region", (string)withRegion.Region())))
            .AsyncReader();
        return await new SearchedRegionCity(reader, withRegion).Read();
    }
}