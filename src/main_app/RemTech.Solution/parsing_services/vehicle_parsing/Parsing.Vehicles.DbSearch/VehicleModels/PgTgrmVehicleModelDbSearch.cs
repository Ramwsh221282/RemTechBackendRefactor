using System.Data.Common;
using Npgsql;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace Parsing.Vehicles.DbSearch.VehicleModels;

public sealed class PgTgrmVehicleModelDbSearch : IVehicleModelDbSearch
{
    private readonly PgConnectionSource _pgConnectionSource;
    private readonly string _sql = string.Intern(
        """
        SELECT text, similarity(@input, text) as sml FROM
        parsed_advertisements_module.vehicle_models
        WHERE similarity(@input, text) >= 0.3
        ORDER BY sml DESC
        LIMIT 1;
        """
    );

    public PgTgrmVehicleModelDbSearch(PgConnectionSource pgConnectionSource)
    {
        _pgConnectionSource = pgConnectionSource;
    }

    public async Task<ParsedVehicleModel> Search(string text)
    {
        await using NpgsqlConnection connection = await _pgConnectionSource.Connect();
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, _sql)).With("@input", text)
            )
        ).AsyncReader();
        return await new SearchedVehicleModel(reader).Read();
    }
}
