using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;

namespace Spares.Infrastructure.Queries.GetSpares;

public sealed class GetSparesQueryHandler(
	NpgSqlSession session,
	EmbeddingsProvider embeddings,
	IOptions<GetSparesThresholdConstants> textSearchConstants
) : IQueryHandler<GetSparesQuery, GetSparesQueryResponse>
{
	private GetSparesThresholdConstants TextSearchConstants { get; } = textSearchConstants.Value;

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

		string sql = $"""
			WITH spares AS (
			    SELECT
			        s.id as spare_id,
			        s.url as spare_url,
			        s.price as spare_price,
			        s.oem as spare_oem,
			        s.text as spare_text,
			        s.type as spare_type,
			        s.is_nds as spare_is_nds,
			        s.region_id as spare_region_id,
			        count(*) over() as total_count,
			        avg(s.price) over() as average_price,
			        max(s.price) over() as maximal_price,
			        min(s.price) over() as minimal_price
			    FROM spares_module.spares s
			    {whereClause}
			    {orderByClause}
			    {paginationClause}
			)
			SELECT s.*, (r.name || ' ' || r.kind) as location FROM spares s
			JOIN vehicles_module.regions r ON s.spare_region_id = r.id
			JOIN contained_items_module.contained_items ci ON s.spare_id = ci.id
			WHERE ci.deleted_at IS NULL			
			""";

		return new CommandDefinition(sql, parameters, transaction: session.Transaction);
	}

	private void ApplyFilters(GetSparesQuery query, DynamicParameters parameters, List<string> filters)
	{
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
			ApplyTextSearch(query.TextSearch, parameters, filters);
		if (!string.IsNullOrWhiteSpace(query.Oem))
			ApplyOemSearch(query.Oem, parameters, filters);
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
			 SELECT kos.id FROM spares_module.spares kos WHERE ts_rank_cd(ts_vector_field, to_tsquery('russian', @oem)) >= @oem_search_threshold
			),
			all_match AS (
			    SELECT coalesce(plain_oem_search.id, keyword_oem_search.id) as id FROM plain_oem_search
			    FULL JOIN keyword_oem_search ON true
			)
			SELECT all_match.id from all_match)
			""";

		filters.Add(sql);
		parameters.Add("@oem", oem, DbType.String);
		parameters.Add("@oem_search_threshold", TextSearchConstants.TextSearchThreshold, DbType.Double);
	}

	private void ApplyTextSearch(string text, DynamicParameters parameters, List<string> filters)
	{
		const string sql = """
			s.id IN (
			WITH embedding_search AS (
			    SELECT
			        id,			        
			        (esv.embedding <-> @embedding_search) AS distance
			    FROM spares_module.spares esv
			    WHERE esv.embedding <-> @embedding_search <= 0.8
			    ORDER BY distance			    
			),
			keyword_search AS (
				SELECT
			    id,
			    text,
			    ts_rank_cd(ts_vector_field, plainto_tsquery('russian', @text_search_parameter)) as ts_rank
			    FROM spares_module.spares ksv
			    WHERE ts_rank_cd(ts_vector_field, plainto_tsquery('russian', @text_search_parameter)) >= 0 OR ksv.text ILIKE '%' || @text_search_parameter || '%'			    
			),
			merged as (
			        SELECT COALESCE(e.id, k.id) as id			            
			        FROM embedding_search e
			        FULL OUTER JOIN keyword_search k ON e.id = k.id
			    )			    
			SELECT id
			FROM merged			
			)
			""";

		filters.Add(sql);
		Vector embedding = new(embeddings.Generate(text));
		parameters.Add("@embedding_search", embedding);
		parameters.Add("@text_search_parameter", text, DbType.String);
	}
}
