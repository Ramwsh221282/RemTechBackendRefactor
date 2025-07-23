using System.Data.Common;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace Parsing.Vehicles.DbSearch.VehicleModels;

public sealed class PgTgrmVehicleModelDbSearch : IVehicleModelDbSearch
{
    private readonly ConnectionSource _connectionSource;
    private readonly string _sql = string.Intern("""
                                                 SELECT text, word_similarity(@input, text) as sml FROM
                                                 parsed_advertisements_module.vehicle_models
                                                 WHERE word_similarity(@input, text) >= 0.9
                                                 ORDER BY sml DESC
                                                 LIMIT 1;
                                                 """);

    public PgTgrmVehicleModelDbSearch(ConnectionSource connectionSource)
    {
        _connectionSource = connectionSource;
    }
    
    public async Task<ParsedVehicleModel> Search(string text)
    {
        await using DbDataReader reader = await new AsyncDbReaderCommand(
                new AsyncPreparedCommand(
                    new ParametrizingPgCommand(
                            new PgCommand(await _connectionSource.Connect(), _sql))
                        .With("@input", text)))
            .AsyncReader();
        return await new SearchedVehicleModel(reader).Read();
    }
}