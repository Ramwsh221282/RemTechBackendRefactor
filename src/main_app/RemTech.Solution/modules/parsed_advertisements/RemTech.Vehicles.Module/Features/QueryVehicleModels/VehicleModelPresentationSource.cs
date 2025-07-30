using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Features.QueryVehicleModels.Delegates;
using RemTech.Vehicles.Module.Features.QueryVehicleModels.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehicleModels;

public static class VehicleModelPresentationSource
{
    private static readonly string Sql = string.Intern(
        """
        SELECT DISTINCT
        m.text as model_name,
        m.id as model_id,
        COUNT(v.id) as vehicles_count
        FROM parsed_advertisements_module.vehicle_models m
        INNER JOIN parsed_advertisements_module.parsed_vehicles v ON m.id = v.model_id
        WHERE v.kind_id = @kind_id
        AND v.brand_id = @brand_id
        GROUP BY m.text, m.id;
        """
    );

    public static async Task<IEnumerable<VehicleModelPresentation>> Provide(
        this NpgsqlConnection connection,
        Guid kindId,
        Guid brandId,
        VehicleModelsCommandSource commandSource,
        VehicleModelsReaderSource readerSource,
        VehicleModelsReadingSource readingSource,
        CancellationToken ct = default
    )
    {
        VehicleModelPresentationReader reader = await readerSource(
            commandSource(connection, kindId, brandId),
            CancellationToken.None
        );
        return await readingSource(reader, ct);
    }

    public static VehicleModelsCommandSource VehicleModelsCommandSource =>
        (connection, kindId, brandId) =>
            new AsyncDbReaderCommand(
                new AsyncPreparedCommand(
                    new ParametrizingPgCommand(new PgCommand(connection, Sql))
                        .With("@kind_id", kindId)
                        .With("@brand_id", brandId)
                )
            );

    public static VehicleModelsReaderSource VehicleModelsReaderSource =>
        async (command, ct) => new VehicleModelPresentationReader(await command.AsyncReader(ct));

    public static VehicleModelsReadingSource VehicleModelsReadingSource =>
        (reader, ct) => reader.ReadAsync(ct);
}
