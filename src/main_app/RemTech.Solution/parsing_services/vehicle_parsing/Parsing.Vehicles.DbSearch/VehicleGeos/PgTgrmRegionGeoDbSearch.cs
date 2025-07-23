using System.Data.Common;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace Parsing.Vehicles.DbSearch.VehicleGeos;

public sealed class PgTgrmRegionGeoDbSearch : IVehicleGeoDbSearch
{
    private readonly ConnectionSource _connectionSource;
    private readonly string _sql = string.Intern("""
                                                 SELECT text, word_similarity(@input, text) as sml
                                                 FROM parsed_advertisements_module.geos
                                                 WHERE word_similarity(@input, text) > 0.8
                                                 ORDER BY sml DESC
                                                 LIMIT 1;
                                                 """);

    public PgTgrmRegionGeoDbSearch(ConnectionSource connectionSource)
    {
        _connectionSource = connectionSource;
    }
    
    public async Task<ParsedVehicleGeo> Search(string text)
    {
        await using DbDataReader reader = await new AsyncDbReaderCommand(
                new AsyncPreparedCommand(
                    new ParametrizingPgCommand(
                            new PgCommand(await _connectionSource.Connect(), _sql))
                        .With("@input", text)))
            .AsyncReader();
        return await new SearchedVehicleGeoOnlyRegion(reader).Read();
    }
}