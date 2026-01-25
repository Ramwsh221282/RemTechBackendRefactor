using System.Data.Common;
using System.Text.Json;
using Dapper;
using Npgsql;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;

namespace ContainedItems.Infrastructure.Queries.GetMainPageItemStats;

public sealed record GetMainPageItemStatsQuery() : IQuery;

public sealed record MainPageItemStatsResponse(
	IReadOnlyList<ItemStats> ItemStats,
	IReadOnlyList<BrandsPopularity> BrandsPopularity,
	IReadOnlyList<CategoriesPopularity> CategoriesPopularity
);

public sealed record ItemStats(string ItemType, int Count);

public sealed record BrandsPopularity(Guid Id, string Name, int Count);

public sealed record CategoriesPopularity(Guid Id, string Name, int Count);

public sealed class GetMainPageItemStatsQueryHandler(NpgSqlSession session)
	: IQueryHandler<GetMainPageItemStatsQuery, MainPageItemStatsResponse>
{
	public async Task<MainPageItemStatsResponse> Handle(GetMainPageItemStatsQuery query, CancellationToken ct = default)
	{
		const string sql = """
			WITH
			item_counts as (
			SELECT
			    jsonb_agg (r) as item_counts
			FROM
			    (
			        SELECT
			            jsonb_build_object ('item_type', ci.creator_type, 'count', COUNT(*)) AS item_stat
			        FROM
			            contained_items_module.contained_items ci
			        WHERE
			            ci.deleted_at IS NULL
			        GROUP BY
			            ci.creator_type
			    ) r
			),
			brands_popularity as (
			SELECT
			    jsonb_agg (r) popular_brands
			FROM
			    (
			        SELECT
			            jsonb_build_object ('id', b.id, 'name', b.name, 'count', COUNT(v.id)) as brands_stat
			        FROM
			            vehicles_module.vehicles v
			            LEFT JOIN vehicles_module.brands b ON v.brand_id = b.id
			        GROUP BY
			            b.id, b.name
			        HAVING
			            COUNT(v.id) > 0
			        ORDER BY
			            COUNT(v.id) DESC
			    ) r
			),
			categories_popularity as (
			SELECT
			    jsonb_agg (r) popular_categories
			FROM
			    (
			        SELECT
			            jsonb_build_object ('id', c.id, 'name', c.name, 'count', COUNT(v.id)) as categories_stat
			        FROM
			            vehicles_module.vehicles v
			            LEFT JOIN vehicles_module.categories c ON v.category_id = c.id
			        GROUP BY
			            c.id, c.name
			        HAVING
			            COUNT(v.id) > 0
			        ORDER BY
			            COUNT(v.id) DESC
			    ) r
			),
			models_popularity as (
			 SELECT jsonb_agg(r) popular_models
			 FROM (
			     SELECT jsonb_build_object(
			            'id', m.id,
			            'name', m.name,
			            'count', COUNT(v.id)
			            ) as models_stat
			     FROM vehicles_module.vehicles v
			     LEFT JOIN vehicles_module.models m ON v.model_id = m.id
			     GROUP BY m.id, m.name
			     HAVING (COUNT(v.id)) > 0
			     ORDER BY
			         COUNT(v.id) DESC
			      ) r
			)
			SELECT
			    ic.item_counts as item_stat,
			    cp.popular_categories as category_stat,
			    bp.popular_brands as brand_stat,
			    mp.popular_models as model_stat
			FROM item_counts ic
			FULL JOIN categories_popularity cp ON TRUE
			FULL JOIN brands_popularity bp ON TRUE
			FULL JOIN models_popularity mp ON TRUE;
			""";

		CommandDefinition command = new(sql, cancellationToken: ct);
		NpgsqlConnection connection = await session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		return await MapToResponse(reader, ct);
	}

	private static async Task<MainPageItemStatsResponse> MapToResponse(DbDataReader reader, CancellationToken ct)
	{
		List<ItemStats> itemStats = [];
		List<BrandsPopularity> brandsPopularities = [];
		List<CategoriesPopularity> categoriesPopularities = [];

		while (await reader.ReadAsync(ct))
		{
			string itemStatJson = reader.GetString(reader.GetOrdinal("item_stat"));
			string brandStatJson = reader.GetString(reader.GetOrdinal("brand_stat"));
			string categoryStatJson = reader.GetString(reader.GetOrdinal("category_stat"));

			ParseItemStatAndFillCollection(itemStatJson, itemStats);
			ParseBrandStatAndFillCollection(brandStatJson, brandsPopularities);
			ParseCategoryStatAndFillCollection(categoryStatJson, categoriesPopularities);
		}

		return new MainPageItemStatsResponse([.. itemStats], [.. brandsPopularities], [.. categoriesPopularities]);
	}

	private static void ParseItemStatAndFillCollection(string itemStatJson, List<ItemStats> collection)
	{
		if (string.IsNullOrEmpty(itemStatJson))
			return;

		using JsonDocument jsonDoc = JsonDocument.Parse(itemStatJson);
		foreach (JsonElement element in jsonDoc.RootElement.EnumerateArray())
		{
			ItemStats? itemStat = ItemFromJson(element.GetProperty("item_stat"));
			if (itemStat != null)
				collection.Add(itemStat);
		}
	}

	private static void ParseBrandStatAndFillCollection(string brandStatJson, List<BrandsPopularity> collection)
	{
		if (string.IsNullOrEmpty(brandStatJson))
			return;

		using JsonDocument jsonDoc = JsonDocument.Parse(brandStatJson);
		foreach (JsonElement element in jsonDoc.RootElement.EnumerateArray())
		{
			BrandsPopularity? brandStat = BrandFromJson(element.GetProperty("brands_stat"));
			if (brandStat != null)
				collection.Add(brandStat);
		}
	}

	private static void ParseCategoryStatAndFillCollection(
		string categoryStatJson,
		List<CategoriesPopularity> collection
	)
	{
		if (string.IsNullOrEmpty(categoryStatJson))
			return;

		using JsonDocument jsonDoc = JsonDocument.Parse(categoryStatJson);
		foreach (JsonElement element in jsonDoc.RootElement.EnumerateArray())
		{
			CategoriesPopularity? categoryStat = CategoryFromJson(element.GetProperty("categories_stat"));
			if (categoryStat != null)
				collection.Add(categoryStat);
		}
	}

	private static CategoriesPopularity? CategoryFromJson(JsonElement element)
	{
		Guid id = element.GetProperty("id").GetGuid();
		string name = element.GetProperty("name").GetString() ?? string.Empty;
		int count = element.GetProperty("count").GetInt32();
		return string.IsNullOrWhiteSpace(name) ? null : new CategoriesPopularity(id, name, count);
	}

	private static BrandsPopularity? BrandFromJson(JsonElement element)
	{
		Guid id = element.GetProperty("id").GetGuid();
		string name = element.GetProperty("name").GetString() ?? string.Empty;
		int count = element.GetProperty("count").GetInt32();
		return string.IsNullOrWhiteSpace(name) ? null : new BrandsPopularity(id, name, count);
	}

	private static ItemStats? ItemFromJson(JsonElement element)
	{
		string itemType = element.GetProperty("item_type").GetString() ?? string.Empty;
		int count = element.GetProperty("count").GetInt32();
		return string.IsNullOrWhiteSpace(itemType) ? null : new ItemStats(itemType, count);
	}
}
