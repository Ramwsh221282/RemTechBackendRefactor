using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.Vehicles.Module.Types.GeoLocations.Decorators.Postgres;

public sealed class PgVehicleGeoFromStoreCommand(string text)
{
    private readonly string _sql = string.Intern(
        """
        SELECT id, text, kind FROM parsed_advertisements_module.geos
        WHERE @text = text
        """
    );

    public async Task<GeoLocation> Fetch(
        NpgsqlConnection connection,
        CancellationToken ct = default
    )
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new OperationException(
                "Нельзя получить геолокацию. Параметр названия локации для поиска пустой."
            );
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, _sql)).With("@text", text)
            )
        ).AsyncReader(ct);
        return await new PgSingleRiddenGeoFromStore(reader).Read();
    }
}
