using System.Data;
using Dapper;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using Vehicles.Infrastructure.Models.Queries.GetModel;

namespace Vehicles.Infrastructure.Models.Queries.GetModels;

public sealed class GetModelsQueryHandler(NpgSqlSession session)
    : IQueryHandler<GetModelsQuery, IEnumerable<ModelResponse>>
{
    public async Task<IEnumerable<ModelResponse>> Handle(
        GetModelsQuery query,
        CancellationToken ct = default
    )
    {
        (DynamicParameters parameters, string sql) = CreateSql(query);
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        return await session.QueryMultipleRows<ModelResponse>(command);
    }

    private static (DynamicParameters parameters, string filterSql) CreateSql(GetModelsQuery query)
    {
        DynamicParameters parameters = new();
        List<string> filterSql = [];
        filterSql.Add("i.deleted_at IS NULL");
        ApplyFilters(query, filterSql, parameters);

        string sql = $"""
            SELECT
                m.id,
                m.name                
            FROM vehicles_module.models m      
            INNER JOIN vehicles_module.vehicles v ON v.model_id = m.id
            INNER JOIN contained_items_module.contained_items i ON v.id = i.id      
            {CreateWhereClause(filterSql)} 
            GROUP BY m.id, m.name
            HAVING COUNT(v.id) > 0                       
            """;

        return (parameters, sql);
    }

    private static void ApplyFilters(
        GetModelsQuery query,
        List<string> filters,
        DynamicParameters parameters
    )
    {
        List<string> subJoins = [];
        List<string> subFilters = [];

        if (HasSomeCategoryFilter(query))
            subJoins.Add("INNER JOIN vehicles_module.categories ic ON ic.id = v.category_id");

        if (HasSomeBrandFilter(query))
            subJoins.Add("INNER JOIN vehicles_module.brands ib ON ib.id = v.brand_id");

        if (query.CategoryId != null && query.CategoryId != Guid.Empty)
        {
            subFilters.Add("ic.id = @category_id");
            parameters.Add("category_id", query.CategoryId, DbType.Guid);
        }

        if (!string.IsNullOrWhiteSpace(query.CategoryName))
        {
            subFilters.Add("ic.name = @category_name");
            parameters.Add("category_name", query.CategoryName, DbType.String);
        }

        if (query.BrandId != null && query.BrandId != Guid.Empty)
        {
            subFilters.Add("ib.id = @brand_id");
            parameters.Add("brand_id", query.BrandId, DbType.Guid);
        }

        if (!string.IsNullOrWhiteSpace(query.BrandName))
        {
            subFilters.Add("ib.name = @brand_name");
            parameters.Add("brand_name", query.BrandName, DbType.String);
        }

        if (subFilters.Count == 0)
            return;

        filters.Add(
            $"""
            EXISTS (
                SELECT 1
                FROM vehicles_module.vehicles v
                {string.Join(" ", subJoins)}
                WHERE v.model_id = m.id
                 AND {string.Join(" AND ", subFilters)})
            """
        );
    }

    private static string CreateWhereClause(List<string> filters) =>
        filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);

    private static bool HasSomeCategoryFilter(GetModelsQuery query) =>
        query.CategoryId != null && query.CategoryId.Value != Guid.Empty
        || !string.IsNullOrWhiteSpace(query.CategoryName);

    private static bool HasSomeBrandFilter(GetModelsQuery query) =>
        query.BrandId != null && query.BrandId.Value != Guid.Empty
        || !string.IsNullOrWhiteSpace(query.BrandName);
}
