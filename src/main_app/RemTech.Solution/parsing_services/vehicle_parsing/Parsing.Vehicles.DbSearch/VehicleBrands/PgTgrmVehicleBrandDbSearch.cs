using System.Data.Common;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace Parsing.Vehicles.DbSearch.VehicleBrands;

public sealed class PgTgrmVehicleBrandDbSearch : IVehicleBrandDbSearch
{
    private readonly ConnectionSource _connectionSource;

    public PgTgrmVehicleBrandDbSearch(ConnectionSource connectionSource)
    {
        _connectionSource = connectionSource;
    }
    
    public async Task<ParsedVehicleBrand> Search(string text)
    {
        string sql = string.Intern("""
                                   SELECT id, text, word_similarity(@input, text) as sml
                                   FROM parsed_advertisements_module.vehicle_brands
                                   WHERE word_similarity(@input, text) > 0.8
                                   ORDER BY sml DESC
                                   LIMIT 1;
                                   """);
        await using DbDataReader reader = await new AsyncDbReaderCommand(
                new AsyncPreparedCommand(
                    new ParametrizingPgCommand(
                            new PgCommand(await _connectionSource.Connect(), sql))
                        .With("@input", text)))
            .AsyncReader();
        return await new SearchedVehicleBrand(reader).Read();
    }
}