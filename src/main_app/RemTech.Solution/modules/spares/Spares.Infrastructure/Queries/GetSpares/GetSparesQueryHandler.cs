using System.Data;
using System.Data.Common;
using System.Text.Json;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;

namespace Spares.Infrastructure.Queries.GetSpares;

/// <summary>
/// Обработчик запроса на получение запчастей.
/// </summary>
public sealed class GetSparesQueryHandler(NpgSqlSession session, EmbeddingsProvider embeddings)
	: IQueryHandler<GetSparesQuery, GetSparesQueryResponse>
{
	/// <summary>
	/// Обрабатывает запрос на получение запчастей.
	/// </summary>
	public async Task<GetSparesQueryResponse> Handle(GetSparesQuery query, CancellationToken ct = default)
	{
		CommandDefinition command = CreateCommand(query);
		await using NpgsqlConnection connection = await session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		return await CreateResponse(reader, ct);
	}

	private static void ApplyPagination(GetSparesQuery query, DynamicParameters parameters, List<string> paginationSql)
	{
		int limit = query.PageSize;
		int offset = (query.Page - 1) * limit;
		paginationSql.Add("LIMIT @limit");
		paginationSql.Add("OFFSET @offset");
		parameters.Add("@limit", limit, DbType.Int32);
		parameters.Add("@offset", offset, DbType.Int32);
	}

	private static void ApplyOrderBy(GetSparesQuery query, List<string> orderBySql)
	{
        if (!string.IsNullOrWhiteSpace(query.TextSearch))
        {
            orderBySql.Add("vector_score DESC");
        }
        
		string orderMode = query.OrderMode;
		if (orderMode == "DESC" || orderMode == "ASC")
		{
			orderBySql.Add($"s.price {orderMode}");
		}
	}

	private static async Task<GetSparesQueryResponse> CreateResponse(DbDataReader reader, CancellationToken ct)
	{
		GetSparesQueryResponse response = new();
		bool aggregatedInfoSet = false;
		List<SpareResponse> spares = [];
		while (await reader.ReadAsync(ct))
		{
			if (!aggregatedInfoSet)
			{
				response.TotalCount = reader.GetInt32(reader.GetOrdinal("total_count"));
				response.AveragePrice = reader.GetDouble(reader.GetOrdinal("average_price"));
				response.MinimalPrice = reader.GetDouble(reader.GetOrdinal("minimal_price"));
				response.MaximalPrice = reader.GetDouble(reader.GetOrdinal("maximal_price"));
				aggregatedInfoSet = true;
			}

            string photosJson = reader.GetString(reader.GetOrdinal("spare_photos"));
            IReadOnlyList<SparePhotoResponse> photosDeserialized = JsonSerializer.Deserialize<IReadOnlyList<SparePhotoResponse>>(photosJson)!;
            IReadOnlyList<string> photosUrlRaw = photosDeserialized.Select(p => p.Value).ToList();
            
			spares.Add(
				new SpareResponse()
				{
					Id = reader.GetGuid(reader.GetOrdinal("spare_id")),
					Url = reader.GetString(reader.GetOrdinal("spare_url")),
					Price = reader.GetInt64(reader.GetOrdinal("spare_price")),
					Oem = reader.GetString(reader.GetOrdinal("spare_oem")),
					Text = reader.GetString(reader.GetOrdinal("spare_text")),
					Type = reader.GetString(reader.GetOrdinal("spare_type")),
					IsNds = reader.GetBoolean(reader.GetOrdinal("spare_is_nds")),
					Location = reader.GetString(reader.GetOrdinal("location")),
                    Photos = photosUrlRaw
				}
			);
		}

		response.Spares = spares;
		return response;
	}

	private CommandDefinition CreateCommand(GetSparesQuery query)
	{
		List<string> filters = [];
		List<string> orderBy = [];
		List<string> pagination = [];
		DynamicParameters parameters = new();
		ApplyFilters(query, parameters, filters);
		ApplyOrderBy(query, orderBy);
		ApplyPagination(query, parameters, pagination);

		string whereClause = filters.Count == 0 ? string.Empty : $" WHERE {string.Join(" AND ", filters)}";
		string orderByClause = orderBy.Count == 0 ? string.Empty : $" ORDER BY {string.Join(", ", orderBy)}";
		string paginationClause = pagination.Count == 0 ? string.Empty : $" {string.Join(" ", pagination)}";
        
        string includeVectorSearchScore = string.IsNullOrWhiteSpace(query.TextSearch)
            ? string.Empty
            : "1 - (s.embedding <=> @embedding_search) as vector_score,";
        
		string sql = $"""
			SELECT
			    {includeVectorSearchScore}
			    s.id as spare_id,
			    s.url as spare_url,
			    s.price as spare_price,
			    o.oem as spare_oem,
			    s.text as spare_text,
			    t.type as spare_type,
			    s.is_nds as spare_is_nds,
			    s.region_id as spare_region_id,
			    s.photos as spare_photos,
			    count(*) over() as total_count,
			    avg(s.price) over() as average_price,
			    max(s.price) over() as maximal_price,
			    min(s.price) over() as minimal_price,
			    (r.name || ' ' || r.kind) as location
			FROM spares_module.spares s
			JOIN spares_module.oems o ON s.oem_id = o.id
			JOIN spares_module.types t ON s.type_id = t.id
			JOIN vehicles_module.regions r ON s.region_id = r.id
			JOIN contained_items_module.contained_items ci ON s.id = ci.id
			{whereClause}
			{orderByClause}
			{paginationClause}
			""";

		return new CommandDefinition(sql, parameters, transaction: session.Transaction);
	}

	private void ApplyFilters(GetSparesQuery query, DynamicParameters parameters, List<string> filters)
	{
        filters.Add("ci.deleted_at IS NULL");
        
		if (!string.IsNullOrWhiteSpace(query.Type))
		{
			filters.Add("s.type=@spare_type");
			parameters.Add("@spare_type", query.Type, DbType.String);
		}

		if (query.RegionId.HasValue)
		{
			filters.Add("s.region_id=@regionId");
			parameters.Add("@regionId", query.RegionId.Value, DbType.Guid);
		}

		if (query.PriceMin.HasValue)
		{
			filters.Add("s.price>=@priceMin");
			parameters.Add("@priceMin", query.PriceMin.Value, DbType.Int64);
		}

		if (query.PriceMax.HasValue)
		{
			filters.Add("s.price<=@priceMax");
			parameters.Add("@priceMax", query.PriceMax.Value, DbType.Int64);
		}

		if (!string.IsNullOrWhiteSpace(query.TextSearch))
		{
			filters.Add("1 - (s.embedding <=> @embedding_search) >= 0.4");
            Vector vector = new(embeddings.Generate(query.TextSearch));
            parameters.Add("@embedding_search", vector);
		}

		if (!string.IsNullOrWhiteSpace(query.Oem))
		{
			ApplyOemSearch(query.Oem, parameters, filters);
		}
	}

	private void ApplyOemSearch(string oem, DynamicParameters parameters, List<string> filters)
	{
		const string sql = """
			s.id IN (
			WITH plain_oem_search AS (
			 SELECT pos.id FROM spares_module.spares pos WHERE pos.oem ILIKE '%' || @oem || '%'
			 LIMIT 1
			),
			keyword_oem_search AS (
			 SELECT kos.id FROM spares_module.spares kos WHERE ts_rank_cd(ts_vector_field, plainto_tsquery('russian', @oem)) >= 0
			),
			all_match AS (
			    SELECT coalesce(plain_oem_search.id, keyword_oem_search.id) as id FROM plain_oem_search
			    FULL JOIN keyword_oem_search ON true
			)
			SELECT all_match.id from all_match)
			""";

		filters.Add(sql);
		parameters.Add("@oem", oem, DbType.String);
	}
}
