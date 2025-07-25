using System.Data.Common;
using Npgsql;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace Parsing.Vehicles.DbSearch.VehicleGeos;

public sealed class PgTgrmRegionCityDbSearch : IVehicleGeoDbSearch
{
    private readonly IVehicleGeoDbSearch _regionSearch;
    private readonly PgConnectionSource _pgConnectionSource;
    private readonly string _sql = string.Intern("""
                                                 SELECT r.text, c.text as text, word_similarity(@input, c.text) as sml
                                                 FROM parsed_advertisements_module.cities c
                                                 INNER JOIN parsed_advertisements_module.geos r ON c.region_id = r.id
                                                 WHERE word_similarity(@input, c.text) > 0.8
                                                 AND r.text = @region
                                                 ORDER BY sml DESC
                                                 LIMIT 1;
                                                 """);
    
    public PgTgrmRegionCityDbSearch(PgConnectionSource pgConnectionSource, IVehicleGeoDbSearch regionSearch)
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