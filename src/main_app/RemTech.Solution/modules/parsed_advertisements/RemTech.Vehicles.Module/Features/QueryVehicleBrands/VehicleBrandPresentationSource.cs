using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Features.QueryVehicleBrands.Delegates;
using RemTech.Vehicles.Module.Features.QueryVehicleBrands.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehicleBrands;

public static class VehicleBrandPresentationSource
{
    private static readonly string Sql = string.Intern(
        """
        SELECT DISTINCT b.text, b.id 
        FROM parsed_advertisements_module.parsed_vehicles v
        INNER JOIN parsed_advertisements_module.vehicle_brands b ON v.brand_id = b.id
        WHERE v.kind_id = @kind_id;
        """
    );

    public static async Task<IEnumerable<VehicleBrandPresentation>> Provide(
        this NpgsqlConnection connection,
        Guid kindId,
        QueryVehicleBrandsCommand commandSource,
        QueriedVehicleBrandsReader readerSource,
        QueriedVehicleBrands brands,
        CancellationToken ct = default
    )
    {
        VehicleBrandPresentationReader reader = await readerSource(
            connection,
            kindId,
            commandSource,
            ct
        );
        return await brands(reader, ct);
    }

    public static QueryVehicleBrandsCommand CreateCommand =>
        (connection, id) =>
            new AsyncDbReaderCommand(
                new AsyncPreparedCommand(
                    new ParametrizingPgCommand(new PgCommand(connection, Sql)).With("@kind_id", id)
                )
            );

    public static QueriedVehicleBrandsReader CreateReader =>
        async (npgsqlConnection, id, source, ct) =>
            new VehicleBrandPresentationReader(await source(npgsqlConnection, id).AsyncReader(ct));

    public static QueriedVehicleBrands ProcessWithReader =>
        async (reader, ct) =>
        {
            LinkedList<VehicleBrandPresentation> presents = [];
            await using (reader.Reader)
            {
                while (await reader.Reader.ReadAsync(ct))
                {
                    Guid id = reader.Reader.GetGuid(reader.Reader.GetOrdinal("id"));
                    string name = reader.Reader.GetString(reader.Reader.GetOrdinal("text"));
                    presents.AddFirst(new VehicleBrandPresentation(id, name));
                }
            }
            return presents.OrderBy(p => p.Name);
        };
}
