using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;

namespace Vehicles.Infrastructure.Locations.Queries;

/// <summary>
/// Обработчик запроса на получение локаций.
/// </summary>
/// <param name="session">Сессия для работы с базой данных PostgreSQL.</param>
/// <param name="embeddings">Провайдер для работы с векторными представлениями.</param>
/// <param name="logger">Логгер для записи логов.</param>
public sealed class GetLocationsQueryHandler(
	NpgSqlSession session,
	EmbeddingsProvider embeddings,
	Serilog.ILogger logger
) : IQueryHandler<GetLocationsQuery, IEnumerable<LocationsResponse>>
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<GetLocationsQuery>();

	/// <summary>
	/// Обрабатывает запрос на получение локаций.
	/// </summary>
	/// <param name="query">Запрос на получение локаций.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Список ответов с информацией о локациях.</returns>
	public async Task<IEnumerable<LocationsResponse>> Handle(GetLocationsQuery query, CancellationToken ct = default)
	{
		(DynamicParameters parameters, string sql) = CreateSql(query);
		CommandDefinition command = new(sql, parameters, cancellationToken: ct);
		NpgsqlConnection connection = await session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		return await MapFromReader(reader, ct);
	}

	private static string AddIncludes(GetLocationsQuery query, Func<GetLocationsQuery, string>[] includeSource)
	{
		string[] includes = [.. includeSource.Select(s => s.Invoke(query)).Where(s => !string.IsNullOrWhiteSpace(s))];
		return includes.Length == 0 ? string.Empty : ", " + string.Join(", ", includes);
	}

	private static string AddOrderBy(GetLocationsQuery query, Func<GetLocationsQuery, string>[] orderBySources)
	{
		string[] orderings = [.. orderBySources.Select(s => s.Invoke(query)).Where(s => !string.IsNullOrWhiteSpace(s))];

		return orderings.Length == 0
			? string.Empty
			: "ORDER BY "
				+ string.Join(
					", ",
					orderBySources.Select(source => source.Invoke(query)).Where(s => !string.IsNullOrWhiteSpace(s))
				);
	}

	private static string CreateLimitClause(GetLocationsQuery query)
	{
		return query.Amount == null ? string.Empty : $"LIMIT {query.Amount.Value}";
	}

	private static void ApplyNotDeletedFilter(List<string> filters)
	{
		filters.Add("ci.deleted_at IS NULL");
	}

	private static void ApplySubQueryFilters(
		GetLocationsQuery query,
		List<string> filters,
		DynamicParameters parameters
	)
	{
		List<string> subJoins = [];
		List<string> subFilters = [];

		ApplyCategoryFilter(query, subJoins, subFilters, parameters);
		ApplyBrandFilter(query, subJoins, subFilters, parameters);
		ApplyModelsFilter(query, subJoins, subFilters, parameters);
		ApplyLocationForSubfilters(query, subFilters, parameters);

		if (subJoins.Count == 0 && subFilters.Count == 0)
		{
			return;
		}

		string join = $"""
			EXISTS (
			    SELECT 1
			    FROM vehicles_module.vehicles iv
			    {string.Join("\n", subJoins)}
			    WHERE iv.region_id = r.id
			    AND {string.Join(" AND ", subFilters)}
			)
			""";

		filters.Add(join);
	}

	private static void ApplyCategoryFilter(
		GetLocationsQuery query,
		List<string> subJoins,
		List<string> subFilters,
		DynamicParameters parameters
	)
	{
		if (!query.ContainsCategoryFilter())
		{
			return;
		}

		subJoins.Add("INNER JOIN vehicles_module.categories c ON iv.category_id = c.id");

		if (query.CategoryId != null && query.CategoryId.Value != Guid.Empty)
		{
			subFilters.Add("c.id = @category_id");
			parameters.Add("@category_id", query.CategoryId.Value, DbType.Guid);
		}

		if (!string.IsNullOrWhiteSpace(query.CategoryName))
		{
			subFilters.Add("c.name = @category_name");
			parameters.Add("@category_name", query.CategoryName, DbType.String);
		}
	}

	private static void ApplyBrandFilter(
		GetLocationsQuery query,
		List<string> subJoins,
		List<string> subFilters,
		DynamicParameters parameters
	)
	{
		if (!query.ContainsBrandFilter())
		{
			return;
		}

		subJoins.Add("INNER JOIN vehicles_module.brands b ON b.id = iv.brand_id");
		if (query.BrandId.HasValue && query.BrandId != Guid.Empty)
		{
			subFilters.Add("b.id = @brand_id");
			parameters.Add("@brand_id", query.BrandId.Value, DbType.Guid);
		}

		if (!string.IsNullOrWhiteSpace(query.BrandName))
		{
			subFilters.Add("b.name = @brand_name");
			parameters.Add("@brand_name", query.BrandName, DbType.String);
		}
	}

	private static void ApplyModelsFilter(
		GetLocationsQuery query,
		List<string> subJoins,
		List<string> subFilters,
		DynamicParameters parameters
	)
	{
		if (!query.ContainsModelFilter())
		{
			return;
		}

		subJoins.Add("INNER JOIN vehicles_module.models m ON m.id = iv.model_id");
		if (query.ModelId.HasValue && query.ModelId != Guid.Empty)
		{
			subFilters.Add("m.id = @model_id");
			parameters.Add("@model_id", query.ModelId.Value, DbType.Guid);
		}

		if (!string.IsNullOrWhiteSpace(query.ModelName))
		{
			subFilters.Add("m.name = @model_name");
			parameters.Add("@model_name", query.ModelName, DbType.String);
		}
	}

	private static void ApplyLocationForSubfilters(
		GetLocationsQuery query,
		List<string> subFilters,
		DynamicParameters parameters
	)
	{
		if (query.Id != null && query.Id.HasValue)
		{
			subFilters.Add("r.location_id = @location_id");
			parameters.Add("@location_id", query.Id.Value, DbType.Guid);
		}
	}

	private static string IncludeTextSearchScore(GetLocationsQuery query)
	{
		return query.ContainsInclude("text-search-score")
			? "r.embedding <-> @embedding AS TextSearchScore"
			: string.Empty;
	}

	private static string IncludeVehiclesCount(GetLocationsQuery query)
	{
		return query.ContainsInclude("vehicles-count") ? "COUNT(v.id) AS VehiclesCount" : string.Empty;
	}

	private static string CreateWhereClause(List<string> filters)
	{
		return filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);
	}

	private static string OrderByLocationName(GetLocationsQuery query)
	{
		if (query.UseOrderByName == null)
		{
			return string.Empty;
		}

		bool use = query.UseOrderByName.Value;
		return use ? "Name ASC" : "Name DESC";
	}

	private static string OrderByTextSearchSimilarity(GetLocationsQuery query)
	{
		return query.ContainsInclude("text-search-score")
			? !string.IsNullOrWhiteSpace(query.TextSearch)
				? "TextSearchScore ASC"
				: string.Empty
			: !string.IsNullOrWhiteSpace(query.TextSearch)
				? "r.embedding <-> @embedding ASC"
				: string.Empty;
	}

	private static bool ReaderContainsField(DbDataReader reader, string fieldName)
	{
		for (int i = 0; i < reader.FieldCount; i++)
		{
			if (reader.GetName(i).Equals(fieldName, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		return false;
	}

	private static async Task<IEnumerable<LocationsResponse>> MapFromReader(DbDataReader reader, CancellationToken ct)
	{
		List<LocationsResponse> responses = [];
		while (await reader.ReadAsync(ct))
		{
			responses.Add(CreateFromReader(reader));
		}

		return responses;
	}

	private static LocationsResponse CreateFromReader(DbDataReader reader)
	{
		return new()
		{
			Id = reader.GetGuid(reader.GetOrdinal("id")),
			Name = reader.GetString(reader.GetOrdinal("name")),
			TextSearchScore = ReaderContainsField(reader, "TextSearchScore")
				? reader.GetFloat(reader.GetOrdinal("TextSearchScore"))
				: null,
			VehiclesCount = ReaderContainsField(reader, "VehiclesCount")
				? reader.GetInt32(reader.GetOrdinal("VehiclesCount"))
				: null,
		};
	}

	private (DynamicParameters Parameters, string Sql) CreateSql(GetLocationsQuery query)
	{
		List<string> filters = [];
		DynamicParameters parameters = new();
		ApplyMainQueryFilters(query, filters, parameters);
		ApplySubQueryFilters(query, filters, parameters);

		string sql = $"""
			SELECT
			  r.id AS Id,
			  r.name || ' ' || r.kind as Name
			  {AddIncludes(query, [IncludeTextSearchScore, IncludeVehiclesCount])}
			  FROM vehicles_module.regions r
			  INNER JOIN vehicles_module.vehicles v ON v.region_id = r.id
			  INNER JOIN contained_items_module.contained_items ci ON ci.id = v.id
			  {CreateWhereClause(filters)}
			  GROUP BY r.id, r.name
			  HAVING COUNT(v.id) > 0              
			  {AddOrderBy(query, [OrderByTextSearchSimilarity, OrderByLocationName])}
			  {CreateLimitClause(query)}
			""";

		Logger.Information(
			"""
			Generated SQL: 
			{Sql}
			""",
			sql
		);

		return (parameters, sql);
	}

	private void ApplyMainQueryFilters(GetLocationsQuery query, List<string> filters, DynamicParameters parameters)
	{
		ApplyNotDeletedFilter(filters);
		ApplyTextSearchFilter(query, filters, parameters);
	}

	private void ApplyTextSearchFilter(GetLocationsQuery query, List<string> filters, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.TextSearch))
		{
			return;
		}

		filters.Add(
			"""
			(   
			    (r.embedding <-> @embedding) <= 0.81 
			    OR r.name ILIKE '%' || @text_search || '%'
			    OR r.kind ILIKE '%' || @text_search || '%'
			    OR (r.name || ' ' || r.kind) ILIKE '%' || @text_search || '%'
			    OR ts_rank_cd(to_tsvector('russian', r.name), plainto_tsquery('russian', @text_search)) > 0
			    OR ts_rank_cd(to_tsvector('russian', r.kind), plainto_tsquery('russian', @text_search)) > 0
			    OR ts_rank_cd(to_tsvector('russian', (r.name || ' ' || r.kind)), plainto_tsquery('russian', @text_search)) > 0
			    )
			"""
		);
		Vector vector = new(embeddings.Generate(query.TextSearch));
		parameters.Add("@embedding", vector);
		parameters.Add("@text_search", query.TextSearch, DbType.String);
	}
}
