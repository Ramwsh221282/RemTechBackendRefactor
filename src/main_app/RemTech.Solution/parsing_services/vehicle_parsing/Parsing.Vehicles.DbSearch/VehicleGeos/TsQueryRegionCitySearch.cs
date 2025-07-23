using System.Data.Common;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace Parsing.Vehicles.DbSearch.VehicleGeos;

public sealed class TsQueryRegionCitySearch : IVehicleGeoDbSearch
{
    private readonly IVehicleGeoDbSearch _regionSearch;
    private readonly ConnectionSource _connectionSource;
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
                    id,
                    text,
                    ts_rank(cities.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                FROM parsed_advertisements_module.cities
                INNER JOIN parsed_advertisements_module.geos ON parsed_advertisements_module.cities.region_id = parsed_advertisements_module.geos.id
                WHERE cities.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                AND parsed_advertisements_module.geos.text = @region
                ORDER BY rank DESC
                LIMIT 1
                ) e ON true
                     LEFT JOIN LATERAL (
                SELECT ts_rank(cities.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                FROM parsed_advertisements_module.cities
                WHERE cities.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                ORDER BY rank DESC
                LIMIT 1
                ) max_sim ON true
            WHERE iw.input_word != '' AND max_sim.rank > 0
            ORDER BY rank DESC
            LIMIT 1;                                     
            """);

    public TsQueryRegionCitySearch(ConnectionSource connectionSource, IVehicleGeoDbSearch regionSearch)
    {
        _regionSearch = regionSearch;
        _connectionSource = connectionSource;
    }
    
    public async Task<ParsedVehicleGeo> Search(string text)
    {
        ParsedVehicleGeo withRegion = await _regionSearch.Search(text);
        if (!withRegion)
            return withRegion;
        await using DbDataReader reader = await new AsyncDbReaderCommand(
                new AsyncPreparedCommand(
                    new ParametrizingPgCommand(
                            new PgCommand(await _connectionSource.Connect(), _sql))
                        .With("@input", text)
                        .With("@region", (string)withRegion.Region())))
            .AsyncReader();
        return await new SearchedRegionCity(reader, withRegion).Read();
    }
}