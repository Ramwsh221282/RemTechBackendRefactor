using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;

namespace Vehicles.Infrastructure.Categories.Queries.GetCategories;

/// <summary>
/// Обработчик запроса на получение категорий транспортных средств.
/// </summary>
/// <param name="session">Сессия базы данных PostgreSQL.</param>
/// <param name="embeddings">Провайдер эмбеддингов для обработки запросов с использованием нейронных сетей.</param>
public sealed class GetCategoriesQueryHandler(NpgSqlSession session, EmbeddingsProvider embeddings)
	: IQueryHandler<GetCategoriesQuery, IEnumerable<CategoryResponse>>
{
	/// <summary>
	/// Обрабатывает запрос на получение категорий транспортных средств.
	/// </summary>
	/// <param name="query">Запрос на получение категорий транспортных средств.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Коллекция ответов с информацией о категориях транспортных средств.</returns>
	public async Task<IEnumerable<CategoryResponse>> Handle(GetCategoriesQuery query, CancellationToken ct = default)
	{
		(DynamicParameters parameters, string sql) = CreateSql(query);
		CommandDefinition command = new(sql, parameters, cancellationToken: ct);
		NpgsqlConnection connection = await session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		return await MapFromReader(reader, ct);
	}

	private static void ApplyQuerySubFilters(
		GetCategoriesQuery query,
		List<string> filters,
		DynamicParameters parameters
	)
	{
		List<string> subFilterJoins = [];
		List<string> subFilters = [];

		if (SomeBrandFilterProvided(query))
		{
			subFilterJoins.Add("INNER JOIN vehicles_module.brands ib ON ib.id = v.brand_id");
		}

		if (query.BrandId.HasValue && query.BrandId != Guid.Empty)
		{
			subFilters.Add("ib.id = @brand_id");
			parameters.Add("brand_id", query.BrandId.Value, DbType.Guid);
		}

		if (!string.IsNullOrWhiteSpace(query.BrandName))
		{
			subFilters.Add("ib.name = @brand_name");
			parameters.Add("brand_name", query.BrandName, DbType.String);
		}

		if (SomeModelFilterProvided(query))
		{
			subFilterJoins.Add("INNER JOIN vehicles_module.models im ON im.id = v.model_id");
		}

		if (query.ModelId.HasValue && query.ModelId != Guid.Empty)
		{
			subFilters.Add("im.id = @model_id");
			parameters.Add("model_id", query.ModelId.Value, DbType.Guid);
		}

		if (!string.IsNullOrWhiteSpace(query.ModelName))
		{
			subFilters.Add("im.name = @model_name");
			parameters.Add("model_name", query.ModelName, DbType.String);
		}

		if (subFilters.Count == 0)
			return;

		filters.Add(
			$"""
			EXISTS (
			    SELECT 1
			    FROM vehicles_module.vehicles v
			    {string.Join(" ", subFilterJoins)}
			    WHERE v.category_id = c.id
			     AND {string.Join(" AND ", subFilters)})
			"""
		);
	}

	private static string ApplyPagination(GetCategoriesQuery query)
	{
		if (query.Page == null || query.PageSize == null)
			return string.Empty;

		int normalizedPage = (query.Page <= 0) ? 1 : query.Page.Value;
		int normalizedPageSize = (query.PageSize >= 50) ? 50 : query.PageSize.Value;
		int offset = (normalizedPage - 1) * normalizedPageSize;
		return $"LIMIT {normalizedPageSize} OFFSET {offset}";
	}

	private static void ApplyMainQueryFilters(
		GetCategoriesQuery query,
		List<string> filters,
		DynamicParameters parameters
	)
	{
		if (query.Id != null && query.Id != Guid.Empty)
		{
			filters.Add("c.id = @category_id");
			parameters.Add("category_id", query.Id, DbType.Guid);
		}

		if (!string.IsNullOrWhiteSpace(query.Name))
		{
			filters.Add("c.name = @category_name");
			parameters.Add("category_name", query.Name, DbType.String);
		}
	}

	private static string ApplyAdditionalFields(GetCategoriesQuery query, Func<GetCategoriesQuery, string>[] includes)
	{
		IEnumerable<string> includedFields = includes
			.Select(i => i.Invoke(query))
			.Where(s => !string.IsNullOrWhiteSpace(s));

		return includedFields.Any() ? ", " + string.Join(", ", includedFields) : string.Empty;
	}

	private static string IncludeTextSearchScore(GetCategoriesQuery query) =>
		query.ContainsIncludedInformationKey("text-search-score")
			? "c.embedding <-> @embedding as text_search_score"
			: string.Empty;

	private static string IncludeCategoriesTotalAmountIfProvided(GetCategoriesQuery query) =>
		query.ContainsIncludedInformationKey("total-categories-count")
			? "COUNT(*) over () as total_categories_count"
			: string.Empty;

	private static string IncludeVehiclesAmountIfProvided(GetCategoriesQuery query) =>
		query.ContainsIncludedInformationKey("vehicles-count") ? "COUNT(v.id) as vehicle_count" : string.Empty;

	private static string CreateWhereClause(List<string> filters) =>
		filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);

	private static bool SomeBrandFilterProvided(GetCategoriesQuery query) =>
		query.BrandId.HasValue || !string.IsNullOrWhiteSpace(query.BrandName);

	private static bool SomeModelFilterProvided(GetCategoriesQuery query) =>
		query.ModelId.HasValue || !string.IsNullOrWhiteSpace(query.ModelName);

	private static async Task<IEnumerable<CategoryResponse>> MapFromReader(DbDataReader reader, CancellationToken ct)
	{
		List<CategoryResponse> responses = [];
		while (await reader.ReadAsync(ct))
		{
			responses.Add(CreateFromReader(reader));
		}

		return responses;
	}

	private static CategoryResponse CreateFromReader(DbDataReader reader) =>
		new()
		{
			Id = reader.GetGuid(reader.GetOrdinal("id")),
			Name = reader.GetString(reader.GetOrdinal("name")),
			VehiclesCount = ContainsColumn(reader, "vehicle_count")
				? reader.GetInt32(reader.GetOrdinal("vehicle_count"))
				: null,
			TextSearchScore = ContainsColumn(reader, "text_search_score")
				? reader.GetFloat(reader.GetOrdinal("text_search_score"))
				: null,
			TotalCategoriesCount = ContainsColumn(reader, "total_categories_count")
				? reader.GetInt32(reader.GetOrdinal("total_categories_count"))
				: null,
		};

	private static bool ContainsColumn(DbDataReader reader, string columnName)
	{
		for (int i = 0; i < reader.FieldCount; i++)
		{
			if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		return false;
	}

	private static string ApplyOrdering(GetCategoriesQuery query, Func<GetCategoriesQuery, string>[] orderings)
	{
		IEnumerable<string> appliedOrderings = orderings
			.Select(o => o.Invoke(query))
			.Where(s => !string.IsNullOrWhiteSpace(s));

		return appliedOrderings.Any() ? "ORDER BY " + string.Join(", ", appliedOrderings) : string.Empty;
	}

	private static string UseOrderByFields(GetCategoriesQuery query)
	{
		if (query.OrderByFields == null)
			return string.Empty;

		string[] fields = [.. query.OrderByFields];
		string orderByMode =
			string.IsNullOrWhiteSpace(query.OrderByMode) ? "ASC"
			: query.OrderByMode == "DESC" ? "DESC"
			: query.OrderByMode == "ASC" ? "ASC"
			: "ASC";

		List<string> orderings = [];
		foreach (string field in fields)
		{
			if (field == "name")
				orderings.Add($"c.name {orderByMode}");
			if (field == "vehicles-count" && query.ContainsIncludedInformationKey("vehicles-count"))
				orderings.Add($"vehicle_count {orderByMode}");
			if (field == "vehicles-count" && !query.ContainsIncludedInformationKey("vehicles-count"))
				orderings.Add($"COUNT(v.id) {orderByMode}");
		}

		return orderings.Count > 0 ? string.Join(", ", orderings) : string.Empty;
	}

	private static string UseEmbeddingsOrderBy(GetCategoriesQuery query)
	{
		return !string.IsNullOrWhiteSpace(query.TextSearch) ? "c.embedding <-> @embedding ASC" : string.Empty;
	}

	private (DynamicParameters parameters, string filterSql) CreateSql(GetCategoriesQuery query)
	{
		DynamicParameters parameters = new();
		List<string> filters = [];
		filters.Add("i.deleted_at IS NULL");
		ApplyFilters(query, filters, parameters);

		string sql = $"""
			SELECT
			c.id as id,
			c.name as name
			{ApplyAdditionalFields(query, [
				IncludeVehiclesAmountIfProvided,
				IncludeTextSearchScore,
				IncludeCategoriesTotalAmountIfProvided,
			])}			
			FROM vehicles_module.categories c                        
			INNER JOIN vehicles_module.vehicles v ON v.category_id = c.id
			INNER JOIN contained_items_module.contained_items i ON v.id = i.id
			{CreateWhereClause(filters)}                        
			GROUP BY c.id, c.name
			HAVING COUNT(v.id) > 0            
			{ApplyOrdering(query, [UseEmbeddingsOrderBy, UseOrderByFields])}
			{ApplyPagination(query)}
			""";

		return (parameters, sql);
	}

	private void ApplyTextSearchFilter(GetCategoriesQuery query, List<string> filters, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.TextSearch))
			return;
		Vector vector = new(embeddings.Generate(query.TextSearch));
		parameters.Add("@embedding", vector);
		parameters.Add("@text_search", query.TextSearch, DbType.String);
		filters.Add(
			"(c.embedding <-> @embedding < 0.81 OR c.name ILIKE '%' || @text_search || '%' OR ts_rank_cd(to_tsvector('russian', c.name), plainto_tsquery('russian', @text_search)) > 0)"
		);
	}

	private void ApplyFilters(GetCategoriesQuery query, List<string> filters, DynamicParameters parameters)
	{
		ApplyMainQueryFilters(query, filters, parameters);
		ApplyQuerySubFilters(query, filters, parameters);
		ApplyTextSearchFilter(query, filters, parameters);
	}
}
