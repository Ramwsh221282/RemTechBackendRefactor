using Categories.Module.Responses;
using Dapper;
using RemTech.Core.Shared.Cqrs;
using Shared.Infrastructure.Module.Postgres;

namespace Categories.Module.Features.QueryPopularCategories;

internal sealed class QueryPopularCategoriesHandler(PostgresDatabase database)
    : ICommandHandler<PopularCategoriesCommand, IEnumerable<CategoryDto>>
{
    private const string Sql = """
        SELECT 
            c.name as name,
            c.id as as id, 
            COUNT(p.id) as items_count 
        FROM categories_module.categories c
        LEFT JOIN parsed_advertisements_module.parsed_vehicles p ON c.id = p.kind_id                             
        GROUP BY c.name, c.id HAVING(COUNT(p.id) > 0)
        ORDER BY vehicles_amount DESC
        LIMIT 6;
        """;

    public async Task<IEnumerable<CategoryDto>> Handle(
        PopularCategoriesCommand command,
        CancellationToken ct = default
    )
    {
        var sqlCommand = new CommandDefinition(Sql, cancellationToken: ct);
        using var connection = await database.ProvideConnection(ct);
        return await connection.QueryAsync<CategoryDto>(sqlCommand);
    }
}
