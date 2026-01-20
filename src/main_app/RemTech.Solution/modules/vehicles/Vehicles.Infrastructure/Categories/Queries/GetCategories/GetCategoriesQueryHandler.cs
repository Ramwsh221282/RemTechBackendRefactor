using System.Data;
using Dapper;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using Vehicles.Infrastructure.Categories.Queries.GetCategory;

namespace Vehicles.Infrastructure.Categories.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler(NpgSqlSession session)
    : IQueryHandler<GetCategoriesQuery, IEnumerable<CategoryResponse>>
{
    public async Task<IEnumerable<CategoryResponse>> Handle(
        GetCategoriesQuery query,
        CancellationToken ct = default
    )
    {
        (DynamicParameters parameters, string sql) = CreateSql(query);
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        return await session.QueryMultipleRows<CategoryResponse>(command);
    }

    private static (DynamicParameters parameters, string filterSql) CreateSql(
        GetCategoriesQuery query
    )
    {
        DynamicParameters parameters = new();
        List<string> filters = [];
        ApplyFilters(query, filters, parameters);

        string sql = $"""
            SELECT
            c.id as id,
            c.name as name
            FROM vehicles_module.categories c            
            {CreateWhereClause(filters)}            
            """;

        return (parameters, sql);
    }

    private static void ApplyFilters(
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

    private static string CreateWhereClause(List<string> filters) =>
        filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);

    private static bool SomeBrandFilterProvided(GetCategoriesQuery query) =>
        query.BrandId.HasValue || !string.IsNullOrWhiteSpace(query.BrandName);

    private static bool SomeModelFilterProvided(GetCategoriesQuery query) =>
        query.ModelId.HasValue || !string.IsNullOrWhiteSpace(query.ModelName);
}
