using System.Data;
using Dapper;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Vehicles.Infrastructure.Models.Queries.GetModel;

public sealed class GetModelQueryHandler(NpgSqlSession session)
    : IQueryHandler<GetModelQuery, ModelResponse?>
{
    public async Task<ModelResponse?> Handle(GetModelQuery query, CancellationToken ct = default)
    {
        if (query.HasNoInitializedProperties())
            return null;

        (DynamicParameters parameters, string sql) = CreateSql(query);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        return await session.QueryMaybeRow<ModelResponse>(command);
    }

    private static (DynamicParameters parameters, string sql) CreateSql(GetModelQuery query)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();
        if (HasModelFiltersProvided(query))
            ApplyModelFilters(query, filters, parameters);
        if (HasCategoryFiltersProvided(query))
            ApplyCategoriesFilters(query, filters, parameters);
        if (HasBrandFiltersProvided(query))
            ApplyBrandFilters(query, filters, parameters);

        string filterSql =
            filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);

        string sql = $"""
            SELECT
            m.id as id,
            m.name as name
            FROM vehicles_module.models m
            {filterSql}
            """;
        return (parameters, sql);
    }

    private static void ApplyModelFilters(
        GetModelQuery query,
        List<string> filters,
        DynamicParameters parameters
    )
    {
        if (query.Id.HasValue && query.Id != Guid.Empty)
        {
            filters.Add("m.id = @model_id");
            parameters.Add("@model_id", query.Id.Value, DbType.Guid);
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            filters.Add("m.name = @model_name");
            parameters.Add("@model_name", query.Name, DbType.String);
        }
    }

    private static void ApplyCategoriesFilters(
        GetModelQuery query,
        List<string> filters,
        DynamicParameters parameters
    )
    {
        List<string> subFilters = [];
        if (query.CategoryId.HasValue && query.CategoryId != Guid.Empty)
        {
            subFilters.Add("c.id = @category_id");
            parameters.Add("@category_id", query.CategoryId.Value, DbType.Guid);
        }

        if (!string.IsNullOrWhiteSpace(query.CategoryName))
        {
            subFilters.Add("c.name = @category_name");
            parameters.Add("@category_name", query.CategoryName, DbType.String);
        }

        if (subFilters.Count == 0)
            return;

        filters.Add(
            $"""
            EXISTS 
            (
                SELECT 1
                FROM vehicles_module.vehicles v
                INNER JOIN vehicles_module.categories c ON c.id = v.category_id
                WHERE v.model_id = m.id AND
                {string.Join(" AND ", subFilters)}
            )
            """
        );
    }

    private static void ApplyBrandFilters(
        GetModelQuery query,
        List<string> filters,
        DynamicParameters parameters
    )
    {
        List<string> brandSubFilters = [];
        if (query.BrandId.HasValue && query.BrandId != Guid.Empty)
        {
            brandSubFilters.Add("b.id = @brand_id");
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
            EXISTS 
            (
                SELECT 1
                FROM vehicles_module.vehicles v
                INNER JOIN vehicles_module.brands b ON b.id = v.brand_id
                WHERE v.model_id = m.id AND
                {string.Join(" AND ", brandSubFilters)}
            )
            """
        );
    }

    private static bool HasBrandFiltersProvided(GetModelQuery query) =>
        query.BrandId != null || !string.IsNullOrWhiteSpace(query.BrandName);

    private static bool HasCategoryFiltersProvided(GetModelQuery query) =>
        query.CategoryId != null || !string.IsNullOrWhiteSpace(query.CategoryName);

    private static bool HasModelFiltersProvided(GetModelQuery query) =>
        query.Id != null || !string.IsNullOrWhiteSpace(query.Name);
}
