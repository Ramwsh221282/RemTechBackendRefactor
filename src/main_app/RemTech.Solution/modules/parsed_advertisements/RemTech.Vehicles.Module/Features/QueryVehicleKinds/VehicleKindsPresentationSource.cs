using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds.Delegates;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehicleKinds;

public static class VehicleKindsPresentationSource
{
    private static readonly string Sql = string.Intern(
        """
        SELECT DISTINCT 
        k.text as kind_name, 
        k.id as kind_id, 
        COUNT(DISTINCT b.id) as brands_count, 
        COUNT(DISTINCT m.id) as models_count, 
        COUNT(v.id) AS vehicles_count
        FROM parsed_advertisements_module.parsed_vehicles v
        INNER JOIN parsed_advertisements_module.vehicle_kinds k ON v.kind_id = k.id
        INNER JOIN parsed_advertisements_module.vehicle_brands b ON  v.brand_id = b.id
        INNER JOIN parsed_advertisements_module.vehicle_models m ON v.model_id = m.id
        GROUP BY k.text, k.id
        """
    );

    public static async Task<IEnumerable<VehicleKindPresentation>> Provide(
        this NpgsqlConnection connection,
        VehicleKindsCommand commandSource,
        VehicleKindsReader readerSource,
        VehicleKindsReading readingSource,
        CancellationToken ct = default
    )
    {
        await using VehicleKindPresentationReader reader = await readerSource(
            commandSource(connection),
            ct
        );
        return await readingSource(reader, ct);
    }

    public static VehicleKindsCommand VehicleKindsCommand =>
        (connection) =>
            new AsyncPreparedCommand(new NoParametrizingPgCommand(new PgCommand(connection, Sql)));

    public static VehicleKindsReader VehicleKindsReader =>
        async (command, ct) =>
            new VehicleKindPresentationReader(
                await new AsyncDbReaderCommand(command).AsyncReader(ct)
            );

    public static VehicleKindsReading VehicleKindsReading =>
        async (reader, ct) => await reader.Read(ct);
}
