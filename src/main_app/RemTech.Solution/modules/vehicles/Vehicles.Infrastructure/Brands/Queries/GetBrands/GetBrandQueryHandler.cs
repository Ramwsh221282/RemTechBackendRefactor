using System.Data;
using Dapper;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using Vehicles.Infrastructure.Brands.Queries.GetBrand;

namespace Vehicles.Infrastructure.Brands.Queries.GetBrands;

public sealed class GetBrandQueryHandler(NpgSqlSession session)
    : IQueryHandler<GetBrandQuery, BrandResponse?>
{
    public async Task<BrandResponse?> Handle(GetBrandQuery query, CancellationToken ct = default)
    {
        if (query.HasNoInitializedProperties())
            return null;
        (DynamicParameters parameters, string sql) = CreateSql(query);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        BrandResponse? category = await session.QueryMaybeRow<BrandResponse>(command);
        return category;
    }

    private static (DynamicParameters parameters, string sql) CreateSql(GetBrandQuery query)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();
        if (BrandFiltersProvided(query))
            ApplyBrandSpecification(query, filters, parameters);
        if (CategoryFiltersProvided(query))
            ApplyCategorySpecification(query, filters, parameters);
        if (ModelFiltersProvided(query))
            ApplyModelSpecification(query, filters, parameters);
        string sql = $"""
            SELECT
            b.id as id,
            b.name as b.name
            FROM vehicles_module.brands b
            {CreateWhereClause(filters)}            
            """;
        return (parameters, sql);
    }

    private static string CreateWhereClause(List<string> filters) =>
        filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);

    private static void ApplyBrandSpecification(
        GetBrandQuery query,
        List<string> filters,
        DynamicParameters parameters
    )
    {
        if (query.BrandId.HasValue && query.BrandId != Guid.Empty)
        {
            filters.Add("b.id = @brand_id");
            parameters.Add("brand_id", query.BrandId);
        }

        if (!string.IsNullOrWhiteSpace(query.BrandName))
        {
            filters.Add("b.name = @brand_name");
            parameters.Add("brand_name", $"%{query.BrandName}%");
        }
    }

    private static void ApplyCategorySpecification(
        GetBrandQuery query,
        List<string> filters,
        DynamicParameters parameters
    )
    {
        List<string> subFilters = [];
        if (query.CategoryId.HasValue && query.CategoryId != Guid.Empty)
        {
            subFilters.Add("c.id = @category_id");
            parameters.Add("category_id", query.CategoryId, DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(query.CategoryName))
        {
            subFilters.Add("c.name = @category_name");
            parameters.Add("category_name", query.CategoryName, DbType.String);
        }

        if (subFilters.Count == 0)
            return;

        filters.Add(
            $"""
            EXISTS (
                SELECT 1
                FROM vehicles_module.vehicles v
                LEFT JOIN vehicles_module.categories c ON v.category_id = c.id
                WHERE v.brand_id = b.id
                AND {string.Join(" AND ", subFilters)}
            )
            """
        );
    }

    private static void ApplyModelSpecification(
        GetBrandQuery query,
        List<string> filters,
        DynamicParameters parameters
    )
    {
        List<string> subFilters = [];
        if (query.ModelId.HasValue && query.ModelId != Guid.Empty)
        {
            subFilters.Add("m.id = @model_id");
            parameters.Add("model_id", query.ModelId, DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(query.ModelName))
        {
            subFilters.Add("m.name = @model_name");
            parameters.Add("model_name", query.ModelName, DbType.String);
        }

        if (subFilters.Count == 0)
            return;

        filters.Add(
            $"""
            EXISTS (
                SELECT 1
                FROM vehicles_module.vehicles v
                LEFT JOIN vehicles_module.models m ON v.model_id = m.id
                WHERE v.brand_id = b.id
                AND {string.Join(" AND ", subFilters)}
            )
            """
        );
    }

    private static bool CategoryFiltersProvided(GetBrandQuery query) =>
        !string.IsNullOrWhiteSpace(query.CategoryName)
        || query.CategoryId.HasValue && query.CategoryId != Guid.Empty;

    private static bool BrandFiltersProvided(GetBrandQuery query) =>
        !string.IsNullOrWhiteSpace(query.BrandName)
        || query.BrandId.HasValue && query.BrandId != Guid.Empty;

    private static bool ModelFiltersProvided(GetBrandQuery query) =>
        !string.IsNullOrWhiteSpace(query.ModelName)
        && query.ModelId.HasValue
        && query.ModelId != Guid.Empty;
}
