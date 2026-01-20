using System.Data;
using Dapper;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using Vehicles.Infrastructure.Brands.Queries.GetBrand;

namespace Vehicles.Infrastructure.Brands.Queries.GetBrands;

public sealed class GetBrandsQueryHandler(NpgSqlSession session)
    : IQueryHandler<GetBrandsQuery, IEnumerable<BrandResponse>>
{
    public async Task<IEnumerable<BrandResponse>> Handle(
        GetBrandsQuery query,
        CancellationToken ct = default
    )
    {
        (DynamicParameters parameters, string sql) = CreateSql(query);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        return await session.QueryMultipleRows<BrandResponse>(command);
    }

    private static (DynamicParameters parameters, string sql) CreateSql(GetBrandsQuery query)
    {
        List<string> filters = [];
        filters.Add("i.deleted_at IS NULL");
        DynamicParameters parameters = new();
        ApplyFilters(query, filters, parameters);

        string sql = $"""
            SELECT
            b.id as Id,
            b.name as Name
            FROM vehicles_module.brands b     
            INNER JOIN vehicles_module.vehicles v ON v.brand_id = b.id
            INNER JOIN contained_items_module.contained_items i ON v.id = i.id       
            {CreateWhereClause(filters)}                                    
            GROUP BY b.id, b.name
            HAVING COUNT(v.id) > 0
            """;

        return (parameters, sql);
    }

    private static void ApplyFilters(
        GetBrandsQuery query,
        List<string> filters,
        DynamicParameters parameters
    )
    {
        List<string> subJoins = [];
        List<string> subFilters = [];

        if (SomeCategoryFilterProvided(query))
            subJoins.Add("INNER JOIN vehicles_module.categories ic ON ic.id = v.category_id");

        if (SomeModelFilterProvided(query))
            subJoins.Add("INNER JOIN vehicles_module.models im ON im.id = v.model_id");

        if (query.CategoryId.HasValue && query.CategoryId != Guid.Empty)
        {
            subFilters.Add("ic.id = @category_id");
            parameters.Add("category_id", query.CategoryId.Value, DbType.Guid);
        }

        if (!string.IsNullOrWhiteSpace(query.CategoryName))
        {
            subFilters.Add("ic.name = @category_name");
            parameters.Add("category_name", query.CategoryName, DbType.String);
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
                {string.Join(" ", subJoins)}
                WHERE v.brand_id = b.id
                AND {string.Join(" AND ", subFilters)}
            )
            """
        );
    }

    private static string CreateWhereClause(List<string> filters) =>
        filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);

    private static bool SomeCategoryFilterProvided(GetBrandsQuery query) =>
        !string.IsNullOrWhiteSpace(query.CategoryName)
        || query.CategoryId.HasValue && query.CategoryId != Guid.Empty;

    private static bool SomeModelFilterProvided(GetBrandsQuery query) =>
        !string.IsNullOrWhiteSpace(query.ModelName)
        || query.ModelId.HasValue && query.ModelId != Guid.Empty;
}
