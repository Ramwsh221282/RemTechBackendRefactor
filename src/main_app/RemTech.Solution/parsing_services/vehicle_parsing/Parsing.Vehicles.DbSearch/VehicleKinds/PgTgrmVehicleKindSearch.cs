using System.Data.Common;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace Parsing.Vehicles.DbSearch.VehicleKinds;

public sealed class PgTgrmVehicleKindSearch : IVehicleKindDbSearch
{
    private readonly ConnectionSource _connectionSource;

    public PgTgrmVehicleKindSearch(ConnectionSource connectionSource)
    {
        _connectionSource = connectionSource;
    }
    
    public async Task<ParsedVehicleKind> Search(string text)
    {
        string sql = string.Intern("""
                                   SELECT id, text, word_similarity(@input, text) as sml FROM parsed_advertisements_module.vehicle_kinds
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
        return await new SearchedVehicleKind(reader).Read();
    }
}