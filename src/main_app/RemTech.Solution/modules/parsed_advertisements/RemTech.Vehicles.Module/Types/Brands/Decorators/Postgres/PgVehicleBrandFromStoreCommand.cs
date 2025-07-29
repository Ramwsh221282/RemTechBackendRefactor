using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Types.Brands.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Brands.Decorators.Postgres;

public sealed class PgVehicleBrandFromStoreCommand(VehicleBrandIdentity identity)
{
    private readonly string _sql = string.Intern(
        """
        SELECT id, text FROM parsed_advertisements_module.vehicle_brands
        WHERE text = @text
        """
    );

    public async Task<VehicleBrand> Fetch(
        NpgsqlConnection connection,
        CancellationToken ct = default
    )
    {
        if (!identity.ReadText())
            throw new OperationException("Параметр Названия бренда для поиска из БД был пустым.");
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, _sql)).With(
                    "@text",
                    (string)identity.ReadText()
                )
            )
        ).AsyncReader(ct);
        return await new PgSingleRiddenVehicleBrand(reader).Read();
    }
}
