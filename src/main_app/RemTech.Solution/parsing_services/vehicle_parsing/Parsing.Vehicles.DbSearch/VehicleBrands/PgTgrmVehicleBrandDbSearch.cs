using System.Data.Common;
using Npgsql;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace Parsing.Vehicles.DbSearch.VehicleBrands;

public sealed class PgTgrmVehicleBrandDbSearch : IVehicleBrandDbSearch
{
    private readonly PgConnectionSource _pgConnectionSource;

    public PgTgrmVehicleBrandDbSearch(PgConnectionSource pgConnectionSource)
    {
        _pgConnectionSource = pgConnectionSource;
    }

    public async Task<ParsedVehicleBrand> Search(string text)
    {
        string sql = string.Intern(
            """
            SELECT id, text, similarity(@input, text) as sml
            FROM parsed_advertisements_module.vehicle_brands
            WHERE similarity(@input, text) > 0.2
            ORDER BY sml DESC
            LIMIT 1;
            """
        );
        await using NpgsqlConnection connection = await _pgConnectionSource.Connect();
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, sql)).With("@input", text)
            )
        ).AsyncReader();
        return await new SearchedVehicleBrand(reader).Read();
    }
}
