using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;

namespace Vehicles.Infrastructure.Categories.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler(NpgSqlSession session, EmbeddingsProvider embeddings)
    : IQueryHandler<GetCategoriesQuery, IEnumerable<CategoryResponse>>
{
    public async Task<IEnumerable<CategoryResponse>> Handle(
        GetCategoriesQuery query,
        CancellationToken ct = default
    )
    {
        (DynamicParameters parameters, string sql) = CreateSql(query);
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        NpgsqlConnection connection = await session.GetConnection(ct);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        return await MapFromReader(reader, ct);
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
            {IncludeVehiclesAmountIfProvided(query)}        
            {IncludeTextSearchScore(query)}
            FROM vehicles_module.categories c                        
            INNER JOIN vehicles_module.vehicles v ON v.category_id = c.id
            INNER JOIN contained_items_module.contained_items i ON v.id = i.id
            {CreateWhereClause(filters)}                        
            GROUP BY c.id, c.name
            HAVING COUNT(v.id) > 0            
            {UseEmbeddingsOrderBy(query)}
            {ApplyPagination(query)}
            """;

        return (parameters, sql);
    }

    private static async Task<IEnumerable<CategoryResponse>> MapFromReader(
        DbDataReader reader,
        CancellationToken ct
    )
    {
        List<CategoryResponse> responses = [];
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetGuid(reader.GetOrdinal("id"));
            string name = reader.GetString(reader.GetOrdinal("name"));
            CategoryResponse response = new(id, name)
            {
                VehiclesCount = ContainsColumn(reader, "vehicle_count")
                    ? reader.GetInt32(reader.GetOrdinal("vehicle_count"))
                    : null,
                TextSearchScore = ContainsColumn(reader, "text_search_score")
                    ? reader.GetFloat(reader.GetOrdinal("text_search_score"))
                    : null,
            };

            responses.Add(response);
        }

        return responses;
    }

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

    private static string UseEmbeddingsOrderBy(GetCategoriesQuery query)
    {
        if (!string.IsNullOrWhiteSpace(query.TextSearch))
            return "ORDER BY c.embedding <-> @embedding ASC";
        return string.Empty;
    }

    private void ApplyTextSearchFilter(
        GetCategoriesQuery query,
        List<string> filters,
        DynamicParameters parameters
    )
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

    private void ApplyFilters(
        GetCategoriesQuery query,
        List<string> filters,
        DynamicParameters parameters
    )
    {
        ApplyMainQueryFilters(query, filters, parameters);
        ApplyQuerySubFilters(query, filters, parameters);
        ApplyTextSearchFilter(query, filters, parameters);
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

    private static string IncludeTextSearchScore(GetCategoriesQuery query) =>
        query.ContainsIncludedInformationKey("text-search-score")
            ? ", c.embedding <-> @embedding as text_search_score"
            : string.Empty;

    private static string IncludeVehiclesAmountIfProvided(GetCategoriesQuery query) =>
        query.ContainsIncludedInformationKey("vehicles-count")
            ? ", COUNT(v.id) as vehicle_count"
            : string.Empty;

    private static string CreateWhereClause(List<string> filters) =>
        filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);

    private static bool SomeBrandFilterProvided(GetCategoriesQuery query) =>
        query.BrandId.HasValue || !string.IsNullOrWhiteSpace(query.BrandName);

    private static bool SomeModelFilterProvided(GetCategoriesQuery query) =>
        query.ModelId.HasValue || !string.IsNullOrWhiteSpace(query.ModelName);
}
