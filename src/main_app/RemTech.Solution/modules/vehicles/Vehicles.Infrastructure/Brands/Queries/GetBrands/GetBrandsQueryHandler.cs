using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;

namespace Vehicles.Infrastructure.Brands.Queries.GetBrands;

/// <summary>
/// Обработчик запроса на получение брендов.
/// </summary>
/// <param name="session">Сессия базы данных PostgreSQL.</param>
/// <param name="embeddings">Провайдер эмбеддингов для обработки текстового поиска.</param>
public sealed class GetBrandsQueryHandler(NpgSqlSession session, EmbeddingsProvider embeddings)
	: IQueryHandler<GetBrandsQuery, IEnumerable<BrandResponse>>
{
	private const StringComparison STRING_COMPARISON = StringComparison.OrdinalIgnoreCase;

	/// <summary>
	/// Обрабатывает запрос на получение брендов.
	/// </summary>
	/// <param name="query">Запрос на получение брендов.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Коллекция ответов с информацией о брендах.</returns>
	public async Task<IEnumerable<BrandResponse>> Handle(GetBrandsQuery query, CancellationToken ct = default)
	{
		(DynamicParameters parameters, string sql) = CreateSql(query);
		CommandDefinition command = session.FormCommand(sql, parameters, ct);
		NpgsqlConnection connection = await session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		return await MapFromReader(reader, ct);
	}

	private static async Task<IEnumerable<BrandResponse>> MapFromReader(DbDataReader reader, CancellationToken ct)
	{
		List<BrandResponse> brands = [];
		while (await reader.ReadAsync(ct))
		{
			brands.Add(CreateFromReader(reader));
		}

		return brands;
	}

	private static BrandResponse CreateFromReader(DbDataReader reader)
	{
		return new()
		{
			Id = reader.GetGuid(reader.GetOrdinal("Id")),
			Name = reader.GetString(reader.GetOrdinal("Name")),
			VehiclesCount = ReaderHasColumn(reader, "VehiclesCount")
				? reader.GetInt32(reader.GetOrdinal("VehiclesCount"))
				: null,
			TextSearchScore = ReaderHasColumn(reader, "TextSearchScore")
				? reader.GetFloat(reader.GetOrdinal("TextSearchScore"))
				: null,
			TotalCount = ReaderHasColumn(reader, "TotalCount")
				? reader.GetInt32(reader.GetOrdinal("TotalCount"))
				: null,
		};
	}

	private static string IncludeAdditionals(GetBrandsQuery query, Func<GetBrandsQuery, string>[] includes)
	{
		string[] normalized = [.. includes.Select(i => i.Invoke(query)).Where(i => !string.IsNullOrWhiteSpace(i))];
		return normalized.Length == 0 ? string.Empty : ", " + string.Join(", ", normalized);
	}

	private static bool ReaderHasColumn(DbDataReader reader, string columnName)
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

	private static string CreateWhereClause(List<string> filters)
	{
		return filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);
	}

	private static bool SomeCategoryFilterProvided(GetBrandsQuery query)
	{
		return !string.IsNullOrWhiteSpace(query.CategoryName)
			|| (query.CategoryId.HasValue && query.CategoryId != Guid.Empty);
	}

	private static bool SomeModelFilterProvided(GetBrandsQuery query)
	{
		return !string.IsNullOrWhiteSpace(query.ModelName) || (query.ModelId.HasValue && query.ModelId != Guid.Empty);
	}

	private static string IncludeTextSearchOrderBy(GetBrandsQuery query)
	{
		return string.IsNullOrWhiteSpace(query.TextSearch) ? string.Empty : "b.embedding <-> @embedding ASC";
	}

	private static string IncludeFieldsOrderBy(GetBrandsQuery query)
	{
		if (query.SortFields is null)
		{
			return string.Empty;
		}

		string orderByMode =
			string.IsNullOrWhiteSpace(query.SortMode) ? "ASC"
			: string.Equals(query.SortMode, "ASC", STRING_COMPARISON) ? "ASC"
			: string.Equals(query.SortMode, "DESC", STRING_COMPARISON) ? "DESC"
			: "ASC";

		string[] orderings = [.. query.SortFields];

		List<string> clauses = [];
		foreach (string clause in orderings)
		{
			if (string.Equals(clause, "name", STRING_COMPARISON))
			{
				clauses.Add($"b.name {orderByMode}");
			}

			if (
				string.Equals(clause, "vehicles-count", STRING_COMPARISON)
				&& query.ContainsFieldInclude("vehicles-count")
			)
			{
				clauses.Add($"VehiclesCount {orderByMode}");
			}

			if (
				string.Equals(clause, "vehicles-count", STRING_COMPARISON)
				&& !query.ContainsFieldInclude("vehicles-count")
			)
			{
				clauses.Add($"COUNT(v.id) {orderByMode}");
			}
		}

		return clauses.Count > 0 ? string.Join(", ", clauses) : string.Empty;
	}

	private static string IncludePagination(GetBrandsQuery query)
	{
		return (query.Page.HasValue && query.PageSize.HasValue)
			? $"LIMIT {query.PageSize.Value} OFFSET {(query.Page.Value - 1) * query.PageSize.Value}"
			: string.Empty;
	}

	private static string IncludeOrderBy(GetBrandsQuery query, Func<GetBrandsQuery, string>[] includes)
	{
		string[] normalized = [.. includes.Select(i => i.Invoke(query)).Where(i => !string.IsNullOrWhiteSpace(i))];

		return normalized.Length == 0 ? string.Empty : "ORDER BY " + string.Join(", ", normalized);
	}

	private static string IncludeVehiclesCount(GetBrandsQuery query)
	{
		return !query.ContainsFieldInclude("vehicles-count") ? string.Empty : "COUNT(v.id) AS VehiclesCount";
	}

	private static string IncludeBrandsTotalCount(GetBrandsQuery query)
	{
		return !query.ContainsFieldInclude("total-brands-count") ? string.Empty : "COUNT(*) OVER() AS TotalCount";
	}

	private static string IncludeVehiclesTextSearchScore(GetBrandsQuery query)
	{
		return !query.ContainsFieldInclude("text-search-score") ? string.Empty : "b.embedding <-> @embedding";
	}

	private static void ApplySubQueryFilters(GetBrandsQuery query, List<string> filters, DynamicParameters parameters)
	{
		List<string> subJoins = [];
		List<string> subFilters = [];

		if (SomeCategoryFilterProvided(query))
		{
			subJoins.Add("INNER JOIN vehicles_module.categories ic ON ic.id = v.category_id");
		}

		if (SomeModelFilterProvided(query))
		{
			subJoins.Add("INNER JOIN vehicles_module.models im ON im.id = v.model_id");
		}

		if (query.CategoryId.HasValue && query.CategoryId != Guid.Empty)
		{
			subFilters.Add("ic.id = @category_id");
			parameters.Add("category_id", query.CategoryId.Value, DbType.Guid);
		}

		if (!string.IsNullOrWhiteSpace(query.CategoryName))
		{
			subFilters.Add("ic.name = @category_name");
			parameters.Add("category_name", query.CategoryName, DbType.String);
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
		{
			return;
		}

		filters.Add(
			$"""
			       EXISTS (
			    SELECT 1
			    FROM vehicles_module.vehicles v
			    {string.Join(" ", subJoins)}
			    WHERE v.brand_id = b.id
			    AND {string.Join(" AND ", subFilters)}
			)
			"""
		);
	}

	private void ApplyFilters(GetBrandsQuery query, List<string> filters, DynamicParameters parameters)
	{
		ApplyMainQueryFilters(query, filters, parameters);
		ApplySubQueryFilters(query, filters, parameters);
	}

	private (DynamicParameters Parameters, string Sql) CreateSql(GetBrandsQuery query)
	{
		List<string> filters = [];
		filters.Add("i.deleted_at IS NULL");
		DynamicParameters parameters = new();

		ApplyFilters(query, filters, parameters);

		string sql = $"""
            SELECT
            b.id as Id,
            b.name as Name
            {IncludeAdditionals(
                query,
                [IncludeVehiclesCount, IncludeVehiclesTextSearchScore, IncludeBrandsTotalCount]
            )}
            FROM vehicles_module.brands b     
            INNER JOIN vehicles_module.vehicles v ON v.brand_id = b.id
            INNER JOIN contained_items_module.contained_items i ON v.id = i.id       
            {CreateWhereClause(filters)}                                    
            GROUP BY b.id, b.name
            HAVING COUNT(v.id) > 0
            {IncludeOrderBy(query, [IncludeTextSearchOrderBy, IncludeFieldsOrderBy])}
            {IncludePagination(query)}
            """;

		return (parameters, sql);
	}

	private void ApplyMainQueryFilters(GetBrandsQuery query, List<string> filters, DynamicParameters parameters)
	{
		if (query.Id != null && query.Id != Guid.Empty)
		{
			filters.Add("b.id = @brand_id");
			parameters.Add("brand_id", query.Id, DbType.Guid);
		}

		if (!string.IsNullOrWhiteSpace(query.Name))
		{
			filters.Add("b.name = @brand_name");
			parameters.Add("brand_name", query.Name, DbType.String);
		}

		if (!string.IsNullOrWhiteSpace(query.TextSearch))
		{
			Vector vector = new(embeddings.Generate(query.TextSearch));
			parameters.Add("@embedding", vector);
			parameters.Add("@text_search", query.TextSearch, DbType.String);
			filters.Add(
				""" 
				(
				b.embedding <-> @embedding <= 0.81
				OR ts_rand_cd(to_tsvector('russian', b.name), plainto_tsquery('russian', @text_search)) > 0 
				OR b.name ILIKE '%' || @text_search || '%'
				)
				"""
			);
		}
	}
}
