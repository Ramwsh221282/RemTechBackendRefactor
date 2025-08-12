using Npgsql;
using NpgsqlTypes;

namespace RemTech.ContainedItems.Module.Features.QueryRecentContainedItems;

internal sealed class SomeVehiclesSource(NpgsqlConnection connection) : ISomeItemsSource
{
    private const string Sql = """
        SELECT
        v.id as vehicle_id,
        v.price as vehicle_price,
        v.is_nds as vehicle_nds,
        v.brand_id as brand_id,
        v.kind_id as kind_id,
        v.model_id as model_id,
        v.geo_id as geo_id,
        v.object as object_data,
        v.description as vehicle_description,
        v.source_url as vehicle_source_url
        FROM parsed_advertisements_module.parsed_vehicles v
        WHERE v.id = ANY(@ids)
        """;

    private const string Vehicle = "Техника";
    private const string IdsParam = "@ids";

    public async Task<IEnumerable<SomeRecentItem>> QueryItems(IEnumerable<QueriedRecentItem> items)
    {
        QueriedRecentItem[] array = items.ToArray();
        if (array.Length == 0 || array.Any(i => i.Type != Vehicle))
            return [];
        List<string> ids = new List<string>(array.Select(item => item.Id));
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = Sql;
        command.Parameters.Add(
            new NpgsqlParameter(IdsParam, NpgsqlDbType.Array | NpgsqlDbType.Text)
            {
                Value = ids.ToArray(),
            }
        );
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        List<RecentVehicle> vehicles = [];
        while (await reader.ReadAsync())
        {
            vehicles.Add(new RecentVehicle(reader));
        }
        return vehicles;
    }
}
