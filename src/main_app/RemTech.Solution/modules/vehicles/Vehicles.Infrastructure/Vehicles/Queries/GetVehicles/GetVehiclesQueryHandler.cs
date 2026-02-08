using System.Data;
using System.Data.Common;
using System.Text.Json;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;

namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

/// <summary>
/// Обработчик запроса на получение транспортных средств.
/// </summary>
// [UseCache]
public sealed class GetVehiclesQueryHandler(NpgSqlSession session, EmbeddingsProvider embeddings)
	: IQueryHandler<GetVehiclesQuery, GetVehiclesQueryResponse>
{
	private NpgSqlSession Session { get; } = session;
	private EmbeddingsProvider Embeddings { get; } = embeddings;

	/// <summary>
	/// Обрабатывает запрос на получение транспортных средств.
	/// </summary>
	public async Task<GetVehiclesQueryResponse> Handle(GetVehiclesQuery query, CancellationToken ct = default)
	{
		(DynamicParameters parameters, string sql) = FormSqlQuery(query, Embeddings);
		CommandDefinition command = new(sql, parameters, cancellationToken: ct);
		await using NpgsqlConnection connection = await Session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		return await FormResponseUsingReader(reader, ct);
	}

	private static (DynamicParameters Parameters, string Sql) FormSqlQuery(
		GetVehiclesQuery query,
		EmbeddingsProvider embeddings
	)
	{
		DynamicParameters parameters = new();
		(parameters, string sql) = FormVehiclesQuery(query.Parameters, parameters, embeddings);
		return (parameters, sql);
	}

	private static async Task<GetVehiclesQueryResponse> FormResponseUsingReader(
		DbDataReader reader,
		CancellationToken ct
	)
	{
		GetVehiclesQueryResponse response = new();
		List<VehicleResponse> vehicles = [];
		while (await reader.ReadAsync(ct))
		{
			response.TotalCount = reader.GetInt32(reader.GetOrdinal("total_count"));
			response.AveragePrice = reader.GetDouble(reader.GetOrdinal("avg_price"));
			response.MinimalPrice = reader.GetDouble(reader.GetOrdinal("min_price"));
			response.MaximalPrice = reader.GetDouble(reader.GetOrdinal("max_price"));
			vehicles.Add(CreateFromReader(reader));
		}

		response.Vehicles = vehicles;
		return response;
	}

	private static VehicleResponse CreateFromReader(DbDataReader reader)
	{
		return new()
		{
			VehicleId = reader.GetGuid(reader.GetOrdinal("vehicle_id")),
			BrandId = reader.GetGuid(reader.GetOrdinal("brand_id")),
			BrandName = reader.GetString(reader.GetOrdinal("brand_name")),
			CategoryId = reader.GetGuid(reader.GetOrdinal("category_id")),
			CategoryName = reader.GetString(reader.GetOrdinal("category_name")),
			ModelId = reader.GetGuid(reader.GetOrdinal("model_id")),
			ModelName = reader.GetString(reader.GetOrdinal("model_name")),
			RegionId = reader.GetGuid(reader.GetOrdinal("region_id")),
			RegionName = reader.GetString(reader.GetOrdinal("region_name")),
			Source = reader.GetString(reader.GetOrdinal("source")),
			Price = reader.GetInt64(reader.GetOrdinal("price")),
			IsNds = reader.GetBoolean(reader.GetOrdinal("is_nds")),
			Text = reader.GetString(reader.GetOrdinal("text")),
			ReleaseYear = reader.IsDBNull(reader.GetOrdinal("release_year"))
				? null
				: reader.GetInt32(reader.GetOrdinal("release_year")),
			Photos = JsonSerializer.Deserialize<string[]>(reader.GetString(reader.GetOrdinal("photos")))!,
			Characteristics = JsonSerializer.Deserialize<VehicleCharacteristicsResponse[]>(
				reader.GetString(reader.GetOrdinal("characteristics"))
			)!,
		};
	}

	private static (DynamicParameters Parameters, string VehiclesCTEQuery) FormVehiclesQuery(
		GetVehiclesQueryParameters queryParameters,
		DynamicParameters parameters,
        EmbeddingsProvider embeddingsProvider
	)
	{
		(parameters, string filterSql) = FormVehiclesFilterSql(queryParameters, parameters, embeddingsProvider);
		(parameters, string orderBySql) = FormVehiclesSortSql(queryParameters, parameters);
		(parameters, string paginationSql) = FormPaginationSql(queryParameters, parameters);
        
        string textSearchInclude = string.IsNullOrWhiteSpace(queryParameters.TextSearch) ? string.Empty : "1 - (v.embedding <=> @embedding_search) as vector_score,";
        
		string sql = $"""
			SELECT
			{textSearchInclude}
			v.id as vehicle_id,
			v.brand_id as brand_id,
			b.name as brand_name,
			v.category_id as category_id,
			c.name as category_name,
			v.model_id as model_id,
			m.name as model_name,
			v.region_id as region_id,
			r.name || ' ' || r.kind as region_name,
			v.source as source,
			v.price as price,
			v.is_nds as is_nds,
			v.text as text,
			v.photos as photos,
			v.characteristics as characteristics,
			(SELECT (elem->>'Value')::int FROM jsonb_array_elements(v.characteristics) AS elem
			WHERE elem->>'Name' = 'Год выпуска'
			LIMIT 1) as release_year,
			count(*) over () as total_count,
			avg(v.price) over () as avg_price,
			min(v.price) over () as min_price,
			max(v.price) over () as max_price
			FROM vehicles_module.vehicles v
			JOIN vehicles_module.brands b ON b.id = v.brand_id
			JOIN vehicles_module.categories c ON c.id = v.category_id
			JOIN vehicles_module.models m ON m.id = v.model_id
			JOIN vehicles_module.regions r ON r.id = v.region_id
			{filterSql}
			{orderBySql}
			{paginationSql}
			""";
		return (parameters, sql);
	}
    
	private static (DynamicParameters Parameters, string PaginationSql) FormPaginationSql(
		GetVehiclesQueryParameters queryParameters,
		DynamicParameters parameters
	)
	{
		int limit = queryParameters.PageSize;
		int offset = (queryParameters.Page - 1) * limit; parameters.Add("@limit", limit, DbType.Int32);
		parameters.Add("@offset", offset, DbType.Int32);
		return (parameters, "LIMIT @limit OFFSET @offset");
	}

	private static (DynamicParameters Parameters, string VehiclesOrderBySql) FormVehiclesSortSql(
		GetVehiclesQueryParameters queryParameters,
		DynamicParameters parameters
	)
	{
		List<string> orderBySql = [];

        if (!string.IsNullOrWhiteSpace(queryParameters.TextSearch))
        {
            orderBySql.Add("vector_score DESC");
        }

        if (queryParameters.SortFields is not null && queryParameters.SortFields.Any())
        {
            string sortMode = queryParameters.Sort switch
            {
                "DESC" => "DESC",
                "ASC" => "ASC",
                _ => string.Empty,
            };
            
            foreach (string field in queryParameters.SortFields)
            {
                string? orderByClause = field switch
                {
                    "price" => $" v.price {sortMode}",
                    "release_year" => $" release_year {OrderByForReleaseYear(sortMode)}",
                    _ => null,
                };
            
                if (orderByClause is not null)
                {
                    orderBySql.Add(orderByClause);
                }
            }
        }

		return orderBySql.Count == 0
			? (parameters, string.Empty)
			: (parameters, " ORDER BY " + string.Join(", ", orderBySql));
	}

	private static string OrderByForReleaseYear(string mode)
	{
		return mode == "ASC" ? "DESC" : "ASC";
	}

	private static (DynamicParameters Parameters, string VehiclesFilterSql) FormVehiclesFilterSql(
        GetVehiclesQueryParameters queryParameters, 
        DynamicParameters parameters,
        EmbeddingsProvider embeddingsProvider)
	{
		List<string> filters = [];
		ApplyVehicleFilters(queryParameters, filters, parameters, embeddingsProvider);
		ApplyCharacteristicsFilter(queryParameters, parameters, filters);
		return filters.Count == 0
			? (parameters, string.Empty)
			: (parameters, $" WHERE {string.Join(" AND ", filters)}");
	}

	private static void ApplyVehicleFilters(
		GetVehiclesQueryParameters queryParameters,
		List<string> filters,
		DynamicParameters parameters,
        EmbeddingsProvider embeddingsProvider
	)
	{
        if (!string.IsNullOrWhiteSpace(queryParameters.TextSearch))
        {
            Vector vector = new(embeddingsProvider.Generate(queryParameters.TextSearch));
            filters.Add("1 - (v.embedding <=> @embedding_search) >= 0.4");
            parameters.Add("@embedding_search", vector);
        }
        
		if (queryParameters.BrandId.HasValue)
		{
			filters.Add("v.brand_id=@brandId");
			parameters.Add("@brandId", queryParameters.BrandId.Value, DbType.Guid);
		}

		if (queryParameters.CategoryId.HasValue)
		{
			filters.Add("v.category_id=@categoryId");
			parameters.Add("@categoryId", queryParameters.CategoryId.Value, DbType.Guid);
		}

		if (queryParameters.RegionId.HasValue)
		{
			filters.Add("v.region_id=@regionId");
			parameters.Add("@regionId", queryParameters.RegionId.Value, DbType.Guid);
		}

		if (queryParameters.ModelId.HasValue)
		{
			filters.Add("v.model_id=@modelId");
			parameters.Add("@modelId", queryParameters.ModelId.Value, DbType.Guid);
		}

		if (queryParameters.IsNds.HasValue)
		{
			filters.Add(
				queryParameters.IsNds.Value switch
				{
					true => "v.is_nds IS true",
					false => "v.is_nds IS false",
				}
			);
		}

		if (queryParameters.MinimalPrice.HasValue)
		{
			filters.Add("v.price >= @minimalPrice");
			parameters.Add("@minimalPrice", queryParameters.MinimalPrice.Value, DbType.Int64);
		}

		if (queryParameters.MaximalPrice.HasValue)
		{
			filters.Add("v.price <= @maximalPrice");
			parameters.Add("@maximalPrice", queryParameters.MaximalPrice.Value, DbType.Int64);
		}
	}

	private static void ApplyCharacteristicsFilter(
		GetVehiclesQueryParameters queryParameters,
		DynamicParameters parameters,
		List<string> filterSql
	)
	{
		if (queryParameters.Characteristics is null)
		{
			return;
		}
		if (queryParameters.Characteristics.Count == 0)
		{
			return;
		}

		string json = JsonSerializer.Serialize(
			queryParameters.Characteristics.Select(c => new { id = c.Key, value = c.Value })
		);
		const string sql = """
			v.id IN (
			    WITH filter_pairs AS (
			    SELECT
			        (f->>'id')::uuid AS ctx_id,
			    f->>'value' as ctx_value
			FROM jsonb_array_elements(@ctxFilters::jsonb) as f
			    )
			SELECT vc.vehicle_id
			    FROM vehicles_module.vehicle_characteristics vc
			JOIN filter_pairs fp ON
			vc.characteristic_id = fp.ctx_id AND
			vc.value = fp.ctx_value
			GROUP BY vc.vehicle_id 
			    HAVING COUNT(*) = (SELECT COUNT(*) FROM filter_pairs)
			    )
			""";
		filterSql.Add(sql);
		parameters.Add("@ctxFilters", json);
	}
}
