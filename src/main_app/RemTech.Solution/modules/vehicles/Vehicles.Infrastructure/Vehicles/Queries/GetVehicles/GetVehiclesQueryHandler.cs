using System.Data;
using System.Data.Common;
using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;

namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

public sealed class GetVehiclesQueryHandler(
	NpgSqlSession session,
	EmbeddingsProvider embeddings,
	IOptions<GetVehiclesThresholdConstants> textSearchOptions
) : IQueryHandler<GetVehiclesQuery, GetVehiclesQueryResponse>
{
	private GetVehiclesThresholdConstants SearchOptions { get; } = textSearchOptions.Value;

	public async Task<GetVehiclesQueryResponse> Handle(GetVehiclesQuery query, CancellationToken ct = default)
	{
		(DynamicParameters parameters, string sql) = FormSqlQuery(query, embeddings, SearchOptions);
		CommandDefinition command = new(sql, parameters, cancellationToken: ct);
		await using NpgsqlConnection connection = await session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		return await FormResponseUsingReader(reader, ct);
	}

	private static (DynamicParameters parameters, string sql) FormSqlQuery(
		GetVehiclesQuery query,
		EmbeddingsProvider embeddings,
		GetVehiclesThresholdConstants searchOptions
	)
	{
		DynamicParameters parameters = new();
		(parameters, string filterSql) = FormVehiclesQuery(query.Parameters, parameters, embeddings, searchOptions);

		string sql = $"""
			WITH vehicles AS (
			    {filterSql}
			)
			SELECT v.*
			FROM vehicles v
			""";

		return (parameters, sql);
	}

	private static async Task<GetVehiclesQueryResponse> FormResponseUsingReader(
		DbDataReader reader,
		CancellationToken ct
	)
	{
		GetVehiclesQueryResponse response = new();
		while (await reader.ReadAsync(ct))
		{
			response.SetTotalCount(reader.GetInt32(reader.GetOrdinal("total_count")));
			response.SetAveragePrice(reader.GetDouble(reader.GetOrdinal("avg_price")));
			response.SetMinimalPrice(reader.GetDouble(reader.GetOrdinal("min_price")));
			response.SetMaximalPrice(reader.GetDouble(reader.GetOrdinal("max_price")));
			response.Vehicles.Add(
				new VehicleResponse()
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
				}
			);
		}

		return response;
	}

	private static (DynamicParameters parameters, string vehiclesCTEQuery) FormVehiclesQuery(
		GetVehiclesQueryParameters queryParameters,
		DynamicParameters parameters,
		EmbeddingsProvider embeddings,
		GetVehiclesThresholdConstants searchOptions
	)
	{
		(parameters, string filterSql) = FormVehiclesFilterSql(queryParameters, parameters, embeddings, searchOptions);
		(parameters, string orderBySql) = FormVehiclesSortSql(queryParameters, parameters);
		(parameters, string paginationSql) = FormPaginationSql(queryParameters, parameters);
		string sql = $"""
			SELECT
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

	private static void ApplySemanticSearchFilter(
		GetVehiclesQueryParameters queryParameters,
		List<string> filterSql,
		EmbeddingsProvider provider,
		DynamicParameters parameters,
		GetVehiclesThresholdConstants searchOptions
	)
	{
		if (string.IsNullOrWhiteSpace(queryParameters.TextSearch))
			return;
		const string sql = """
			v.id IN (
			WITH embedding_search AS (
			    SELECT
			        id,
			        text,
			        (esv.embedding <=> @embedding_search) AS distance
			    FROM vehicles_module.vehicles esv
			    WHERE esv.embedding <=> @embedding_search <= @embedding_search_threshold
			    ORDER BY distance
			    LIMIT 50
			),
			    keyword_search AS (
			    SELECT
			    id,
			    text,
			    ts_rank_cd(ts_vector_field, plainto_tsquery('russian', @text_search_parameter)) as ts_rank
			    FROM vehicles_module.vehicles ksv
			    WHERE ts_rank_cd(ts_vector_field, plainto_tsquery('russian', @text_search_parameter)) > @text_search_threshold
			    ORDER BY ts_rank DESC
			    LIMIT 50
			    ),
			    merged as (
			        SELECT
			            COALESCE(e.id, k.id) as id,
			            COALESCE(e.text, k.text) as text,
			            e.distance,
			            k.ts_rank
			        FROM embedding_search e
			        FULL OUTER JOIN keyword_search k ON e.id = k.id
			    ),
			    scored AS (
			        SELECT
			            id,
			            text,
			            distance,
			            ts_rank,
			        (
			            COALESCE(1.0 - distance, 0.0) * 0.6 +
			            COALESCE(ts_rank, 0.0) * 0.4
			            ) as hybrid_score
			        FROM merged
			    )
			SELECT id
			FROM scored
			WHERE hybrid_score > @hybrid_threshold
			)
			""";
		filterSql.Add(sql);
		Vector embedding = new(provider.Generate(queryParameters.TextSearch));
		parameters.Add("@embedding_search", embedding);
		parameters.Add("@embedding_search_threshold", searchOptions.EmbeddingSearchThreshold, DbType.Double);
		parameters.Add("@text_search_parameter", queryParameters.TextSearch, DbType.String);
		parameters.Add("@text_search_threshold", searchOptions.TextSearchThreshold, DbType.Double);
		parameters.Add("@hybrid_threshold", searchOptions.HybridSearchThreshold, DbType.Double);
	}

	private static (DynamicParameters parameters, string paginationSql) FormPaginationSql(
		GetVehiclesQueryParameters queryParameters,
		DynamicParameters parameters
	)
	{
		int limit = queryParameters.PageSize;
		int offset = (queryParameters.Page - 1) * limit;
		parameters.Add("@limit", limit, DbType.Int32);
		parameters.Add("@offset", offset, DbType.Int32);
		return (parameters, $"LIMIT @limit OFFSET @offset");
	}

	private static (DynamicParameters parameters, string vehiclesOrderBySql) FormVehiclesSortSql(
		GetVehiclesQueryParameters queryParameters,
		DynamicParameters parameters
	)
	{
		List<string> orderBySql = [];
		if (queryParameters.Sort == "NONE")
			return (parameters, string.Empty);
		if (queryParameters.SortFields is null)
			return (parameters, string.Empty);
		if (queryParameters.SortFields.Any() == false)
			return (parameters, string.Empty);
		string? sortMode = queryParameters.Sort switch
		{
			"DESC" => "DESC",
			"ASC" => "ASC",
			_ => null,
		};
		if (sortMode is null)
			return (parameters, string.Empty);

		foreach (string field in queryParameters.SortFields)
		{
			string? orderByClause = field switch
			{
				"price" => $" v.price {sortMode}",
				"release_year" => $" release_year {OrderByForReleaseYear(sortMode)}",
				_ => null,
			};
			if (orderByClause is not null)
				orderBySql.Add(orderByClause);
		}

		return orderBySql.Count == 0
			? (parameters, string.Empty)
			: (parameters, " ORDER BY " + string.Join(", ", orderBySql));

		static string OrderByForReleaseYear(string mode) => mode == "ASC" ? "DESC" : "ASC";
	}

	private static (DynamicParameters parameters, string vehiclesFilterSql) FormVehiclesFilterSql(
		GetVehiclesQueryParameters queryParameters,
		DynamicParameters parameters,
		EmbeddingsProvider embeddings,
		GetVehiclesThresholdConstants searchOptions
	)
	{
		List<string> filters = [];
		ApplyVehicleFilters(queryParameters, filters, parameters);
		ApplyCharacteristicsFilter(queryParameters, parameters, filters);
		ApplySemanticSearchFilter(queryParameters, filters, embeddings, parameters, searchOptions);
		return filters.Count == 0
			? (parameters, string.Empty)
			: (parameters, $" WHERE {string.Join(" AND ", filters)}");
	}

	private static void ApplyVehicleFilters(
		GetVehiclesQueryParameters queryParameters,
		List<string> filters,
		DynamicParameters parameters
	)
	{
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
			return;
		if (queryParameters.Characteristics.Count == 0)
			return;
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
