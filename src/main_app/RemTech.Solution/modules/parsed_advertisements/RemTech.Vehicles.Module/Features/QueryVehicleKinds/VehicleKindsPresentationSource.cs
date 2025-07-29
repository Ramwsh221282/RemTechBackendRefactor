using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds.Delegates;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehicleKinds;

public static class VehicleKindsPresentationSource
{
    private static readonly string Sql = string.Intern(
        """
        SELECT DISTINCT k.text, k.id
        FROM parsed_advertisements_module.parsed_vehicles v
        INNER JOIN parsed_advertisements_module.vehicle_kinds k ON v.kind_id = k.id
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
        VehicleKindPresentationReader reader = await readerSource(commandSource(connection), ct);
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
        async (reader, ct) =>
        {
            LinkedList<VehicleKindPresentation> presents = [];
            await using (reader.Reader)
            {
                while (await reader.Reader.ReadAsync(ct))
                {
                    string text = reader.Reader.GetString(reader.Reader.GetOrdinal("text"));
                    Guid id = reader.Reader.GetGuid(reader.Reader.GetOrdinal("id"));
                    presents.AddFirst(new VehicleKindPresentation(id, text));
                }
            }
            return presents.OrderBy(v => v.Name);
        };
}
