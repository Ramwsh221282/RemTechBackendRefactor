using System.Data.Common;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace Parsing.Vehicles.DbSearch.VehicleGeos;

public sealed class PgTgrmRegionCityDbSearch : IVehicleGeoDbSearch
{
    private readonly IVehicleGeoDbSearch _regionSearch;
    private readonly ConnectionSource _connectionSource;
    private readonly string _sql = string.Intern("""
                                                 SELECT r.text, c.text as city, word_similarity(@input, c.text) as sml
                                                 FROM parsed_advertisements_module.cities c
                                                 WHERE word_similarity(@input, c.text) > 0.8
                                                 AND r.text = @region
                                                 INNER JOIN parsed_advertisements_module.geos r ON
                                                 r.id = c.region_id
                                                 ORDER BY sml DESC
                                                 LIMIT 1;
                                                 """);
    
    public PgTgrmRegionCityDbSearch(ConnectionSource connectionSource, IVehicleGeoDbSearch regionSearch)
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