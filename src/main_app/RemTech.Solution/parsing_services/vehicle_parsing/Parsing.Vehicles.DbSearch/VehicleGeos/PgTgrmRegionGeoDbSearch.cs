using System.Data.Common;
using Npgsql;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace Parsing.Vehicles.DbSearch.VehicleGeos;

public sealed class PgTgrmRegionGeoDbSearch : IVehicleGeoDbSearch
{
    private readonly PgConnectionSource _pgConnectionSource;
    private readonly string _sql = string.Intern(
        """
        SELECT text, similarity(@input, text) as sml
        FROM parsed_advertisements_module.geos
        WHERE similarity(@input, text) > 0.2
        ORDER BY sml DESC
        LIMIT 1;
        """
    );

    public PgTgrmRegionGeoDbSearch(PgConnectionSource pgConnectionSource)
    {
        _pgConnectionSource = pgConnectionSource;
    }

    public async Task<ParsedVehicleGeo> Search(string text)
    {
        await using NpgsqlConnection connection = await _pgConnectionSource.Connect();
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, _sql)).With("@input", text)
            )
        ).AsyncReader();
        return await new SearchedVehicleGeoOnlyRegion(reader).Read();
    }
}
