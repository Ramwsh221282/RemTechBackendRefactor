using System.Data;
using Dapper;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Vehicles.Infrastructure.Categories.Queries.GetCategory;

public sealed class GetCategoryQueryHandler(NpgSqlSession session)
    : IQueryHandler<GetCategoryQuery, CategoryResponse?>
{
    public async Task<CategoryResponse?> Handle(
        GetCategoryQuery query,
        CancellationToken ct = default
    )
    {
        (DynamicParameters parameters, string sql) = CreateSql(query);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        return await session.QueryMaybeRow<CategoryResponse>(command);
    }

    private static (DynamicParameters, string sql) CreateSql(GetCategoryQuery query)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();

        if (CategoryFiltersProvided(query))
            ApplyCategoryFiltersSpecification(query, filters, parameters);
        if (BrandFiltersProvided(query))
            ApplyBrandFiltersSpecification(query, filters, parameters);
        if (ModelFiltersProvided(query))
            ApplyModelFiltersSpecification(query, filters, parameters);

        string filterSql = CreateFilterSql(filters);
        string sql = $"""
            SELECT 
            c.id as id,
            c.name as name
            FROM vehicles_module.categories c
            {filterSql}
            """;
        return (parameters, sql);
    }

    private static string CreateFilterSql(List<string> filters) =>
        filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);

    private static bool BrandFiltersProvided(GetCategoryQuery query) =>
        !string.IsNullOrWhiteSpace(query.CategoryName)
        || query.BrandId.HasValue && query.BrandId != Guid.Empty;

    private static bool ModelFiltersProvided(GetCategoryQuery query) =>
        !string.IsNullOrWhiteSpace(query.ModelName)
        || query.ModelId.HasValue && query.ModelId != Guid.Empty;

    private static void ApplyModelFiltersSpecification(
        GetCategoryQuery query,
        List<string> filters,
        DynamicParameters parameters
    )
    {
        List<string> modelSubFilters = [];
        if (query.ModelId.HasValue && query.ModelId != Guid.Empty)
        {
            modelSubFilters.Add("v.model_id = @model_id");
            parameters.Add("@model_id", query.ModelId.Value, DbType.Guid);
        }

        if (!string.IsNullOrWhiteSpace(query.ModelName))
        {
            modelSubFilters.Add("m.name = @model_name");
            parameters.Add("@model_name", query.ModelName, DbType.String);
        }

        if (modelSubFilters.Count == 0)
            return;

        filters.Add(
            $"""
            EXISTS (
                SELECT 1
                FROM vehicles_module.vehicles v
                INNER JOIN vehicles_module.models m ON m.id = v.model_id
                WHERE v.category_id = c.id AND
                {string.Join(" AND ", modelSubFilters)}                
            )
            """
        );
    }

    private static void ApplyBrandFiltersSpecification(
        GetCategoryQuery query,
        List<string> filters,
        DynamicParameters parameters
    )
    {
        List<string> brandSubFilters = [];
        if (query.BrandId.HasValue && query.BrandId != Guid.Empty)
        {
            brandSubFilters.Add("v.brand_id = @brand_id");
            parameters.Add("@brand_id", query.BrandId.Value, DbType.Guid);
        }

        if (!string.IsNullOrWhiteSpace(query.BrandName))
        {
            brandSubFilters.Add("b.name = @brand_name");
            parameters.Add("@brand_name", query.BrandName, DbType.String);
        }

        if (brandSubFilters.Count == 0)
            return;

        filters.Add(
            $"""
            EXISTS (
                SELECT 1
                FROM vehicles_module.vehicles v
                INNER JOIN vehicles_module.brands b ON b.id = v.brand_id
                WHERE v.category_id = c.id AND
                {string.Join(" AND ", brandSubFilters)}
            )
            """
        );
    }

    private static bool CategoryFiltersProvided(GetCategoryQuery query) =>
        !string.IsNullOrWhiteSpace(query.CategoryName)
        || query.CategoryId.HasValue && query.CategoryId != Guid.Empty;

    private static void ApplyCategoryFiltersSpecification(
        GetCategoryQuery query,
        List<string> filters,
        DynamicParameters parameters
    )
    {
        if (!string.IsNullOrWhiteSpace(query.CategoryName))
        {
            filters.Add("c.name = @category_name");
            parameters.Add("@category_name", query.CategoryName, DbType.String);
        }

        if (query.CategoryId != null && query.CategoryId.HasValue && query.CategoryId != Guid.Empty)
        {
            filters.Add("c.id = @category_id");
            parameters.Add("@category_id", query.CategoryId.Value, DbType.Guid);
        }
    }
}
