using System.Data.Common;
using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;

namespace RemTech.Vehicles.Module.Features.QueryConcreteVehicle;

internal sealed class ConcreteVehicleQuery
{
    private const string Sql = """
        SELECT
        v.id as vehicle_id,
        v.price as vehicle_price,
        v.is_nds as vehicle_nds,
        v.brand_id as brand_id,
        v.kind_id as category_id,
        v.model_id as model_id,
        v.geo_id as region_id,
        v.source_url as vehicle_source_url,
        v.object as vehicle_object_data,
        v.description as vehicle_description
        FROM parsed_advertisements_module.parsed_vehicles v
        WHERE v.id = @id;
        """;

    public async Task<VehiclePresentation?> Query(
        string id,
        NpgsqlConnection connection,
        CancellationToken ct = default
    )
    {
        if (string.IsNullOrEmpty(id))
            return null;
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = Sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@id", id));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return null;
        await reader.ReadAsync(ct);
        VehiclePresentation presentation = VehiclePresentation.FromReader(reader);
        return presentation;
    }
}
