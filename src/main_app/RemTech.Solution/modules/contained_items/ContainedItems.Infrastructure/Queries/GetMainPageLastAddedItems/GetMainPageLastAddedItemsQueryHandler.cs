using System.Data.Common;
using System.Text.Json;
using Dapper;
using Npgsql;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;

namespace ContainedItems.Infrastructure.Queries.GetMainPageLastAddedItems;

public sealed class GetMainPageLastAddedItemsQueryHandler(NpgSqlSession session)
	: IQueryHandler<GetMainPageLastAddedItemsQuery, MainPageLastAddedItemsResponse>
{
	public static MainPageLastAddedItem CreateAsSpare(SpareData spare) => new(spare, null);

	public static MainPageLastAddedItem CreateAsVehicle(VehicleData vehicle) => new(null, vehicle);

	public async Task<MainPageLastAddedItemsResponse> Handle(
		GetMainPageLastAddedItemsQuery query,
		CancellationToken ct = default
	)
	{
		const string sql = """
			    WITH vehicle_items AS (
			    SELECT
			        jsonb_build_object(
			            'id', v.id,
			            'title', c.name || ' ' || b.name || ' ' || m.name,
			            'photos', v.photos,
			            'characteristics', (
			                SELECT jsonb_agg(
			                    jsonb_build_object('characteristic', ch.name, 'value', vc."value")
			                )
			                FROM vehicles_module.vehicle_characteristics vc
			                LEFT JOIN vehicles_module.characteristics ch ON ch.id = vc.characteristic_id
			                WHERE vc.vehicle_id = v.id
			            ),
			            'type', 'vehicle'
			        ) AS item,
			        ci.created_at as created_at
			    FROM contained_items_module.contained_items ci
			    LEFT JOIN vehicles_module.vehicles v ON v.id = ci.id
			    LEFT JOIN vehicles_module.brands b ON b.id = v.brand_id
			    LEFT JOIN vehicles_module.categories c ON c.id = v.category_id
			    LEFT JOIN vehicles_module.models m ON m.id = v.model_id
			    WHERE ci.deleted_at IS NULL
			    AND v.id IS NOT NULL
			    ORDER BY ci.created_at DESC
			    LIMIT 25
			),
			spare_items AS (
			    SELECT
			        jsonb_build_object(
			            'id', s.id,
			            'title', s."type" || ' ' || s.oem || ' ' || s.text,
			            'type', 'spare'
			        ) AS item,
			        ci.created_at as created_at
			    FROM contained_items_module.contained_items ci
			    LEFT JOIN spares_module.spares s ON s.id = ci.id
			    WHERE ci.deleted_at IS NULL
			    AND s.id IS NOT NULL
			    ORDER BY ci.created_at DESC
			    LIMIT 25
			)

			SELECT NULL::jsonb as vehicle, NULL::jsonb as spare
			UNION ALL
			SELECT item as vehicle, NULL::jsonb as spare
			FROM vehicle_items
			UNION ALL
			SELECT NULL::jsonb as vehicle, item as spare
			FROM spare_items
			OFFSET 1           
			""";

		CommandDefinition command = new(sql, cancellationToken: ct);
		NpgsqlConnection connection = await session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		return await MapToResponse(reader, ct);
	}

	private static async Task<MainPageLastAddedItemsResponse> MapToResponse(DbDataReader reader, CancellationToken ct)
	{
		List<MainPageLastAddedItem> items = new(50);
		while (await reader.ReadAsync(ct))
		{
			int vehicleDataPosition = reader.GetOrdinal("vehicle");
			int spareDataPosition = reader.GetOrdinal("spare");

			if (!await reader.IsDBNullAsync(vehicleDataPosition, ct))
				ParseAndAddVehicleItem(reader.GetString(vehicleDataPosition), items);

			if (!await reader.IsDBNullAsync(spareDataPosition, ct))
				ParseAndAddSpareItem(reader.GetString(spareDataPosition), items);
		}

		return new MainPageLastAddedItemsResponse(items);
	}

	private static void ParseAndAddSpareItem(string spareDataJson, List<MainPageLastAddedItem> collection)
	{
		if (string.IsNullOrWhiteSpace(spareDataJson))
			return;

		using JsonDocument doc = JsonDocument.Parse(spareDataJson);
		Guid id = doc.RootElement.GetProperty("id").GetGuid();
		string? title = doc.RootElement.GetProperty("title").GetString();
		if (!string.IsNullOrWhiteSpace(title))
			collection.Add(CreateAsSpare(new SpareData(id, title)));
	}

	private static void ParseAndAddVehicleItem(string vehicleDataJson, List<MainPageLastAddedItem> collection)
	{
		if (string.IsNullOrWhiteSpace(vehicleDataJson))
			return;

		using JsonDocument doc = JsonDocument.Parse(vehicleDataJson);
		Guid id = doc.RootElement.GetProperty("id").GetGuid();
		string? title = doc.RootElement.GetProperty("title").GetString();
		if (string.IsNullOrWhiteSpace(title))
			return;
		string[]? photos = JsonSerializer.Deserialize<string[]>(doc.RootElement.GetProperty("photos"));
		photos ??= [];
		List<VehicleDataCharacteristic> characteristics = [];

		foreach (JsonElement element in doc.RootElement.GetProperty("characteristics").EnumerateArray())
		{
			string value = element.GetProperty("value").GetString() ?? string.Empty;
			if (string.IsNullOrWhiteSpace(value))
				continue;
			string characteristic = element.GetProperty("characteristic").GetString() ?? string.Empty;
			if (string.IsNullOrWhiteSpace(characteristic))
				continue;
			characteristics.Add(new VehicleDataCharacteristic(characteristic, value));
		}

		collection.Add(CreateAsVehicle(new VehicleData(id, title, photos, characteristics)));
	}
}
