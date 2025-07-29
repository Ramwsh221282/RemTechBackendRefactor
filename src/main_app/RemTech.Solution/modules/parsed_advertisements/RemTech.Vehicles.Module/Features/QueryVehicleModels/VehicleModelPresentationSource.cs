using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Features.QueryVehicleModels.Delegates;
using RemTech.Vehicles.Module.Features.QueryVehicleModels.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehicleModels;

public static class VehicleModelPresentationSource
{
    private static readonly string Sql = string.Intern(
        """
        SELECT DISTINCT m.text, m.id
        FROM parsed_advertisements_module.vehicle_models m
        INNER JOIN parsed_advertisements_module.parsed_vehicles v ON m.id = v.model_id
        WHERE
            v.kind_id = @kind_id
        AND
            v.brand_id = @brand_id
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
        async (reader, ct) =>
        {
            LinkedList<VehicleModelPresentation> presents = [];
            await using (reader.Reader)
            {
                while (await reader.Reader.ReadAsync(ct))
                {
                    Guid id = reader.Reader.GetGuid(reader.Reader.GetOrdinal("id"));
                    string text = reader.Reader.GetString(reader.Reader.GetOrdinal("text"));
                    presents.AddFirst(new VehicleModelPresentation(id, text));
                }
            }

            return presents.OrderBy(x => x.Name);
        };
}
