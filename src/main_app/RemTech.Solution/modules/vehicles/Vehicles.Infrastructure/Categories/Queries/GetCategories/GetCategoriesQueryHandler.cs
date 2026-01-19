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
        string sql = CreateSql();
        CommandDefinition command = new(sql, cancellationToken: ct);
        return await session.QueryMultipleRows<CategoryResponse>(command);
    }

    private string CreateSql()
    {
        return """
            SELECT
            c.id as id,
            c.name as name
            FROM vehicles_module.categories c
            LEFT JOIN vehicles_module.vehicles v ON v.category_id = c.id
            GROUP BY c.id, c.name
            HAVING COUNT(v.id) > 0
            """;
    }
}
