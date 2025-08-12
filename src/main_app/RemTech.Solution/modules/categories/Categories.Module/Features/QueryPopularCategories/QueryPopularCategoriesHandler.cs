using Npgsql;
using Shared.Infrastructure.Module.Cqrs;
using Shared.Infrastructure.Module.Postgres.PgCommandsBetter;

namespace Categories.Module.Features.QueryPopularCategories;

internal sealed class QueryPopularCategoriesHandler(NpgsqlConnection connection)
    : ICommandHandler<PopularCategoriesCommand, IEnumerable<PopularCategoriesResponse>>
{
    private const string Sql = """
        SELECT c.name as category_name, c.id as category_id, COUNT(p.id) as vehicles_amount FROM categories_module.categories c
        LEFT JOIN parsed_advertisements_module.parsed_vehicles p ON c.id = p.kind_id                             
        GROUP BY c.name, c.id HAVING(COUNT(p.id) > 0)
        ORDER BY vehicles_amount DESC
        LIMIT 6;
        """;
    private const string CategoryNameColumn = "category_name";
    private const string CategoryIdColumn = "category_id";

    public async Task<IEnumerable<PopularCategoriesResponse>> Handle(
        PopularCategoriesCommand command,
        CancellationToken ct = default
    ) =>
        await connection
            .Command()
            .WithQueryString(Sql)
            .ClosureReader(ct)
            .AsBlockOf<PopularCategoriesResponse>()
            .Map(r => new PopularCategoriesResponse(
                r.GetString(r.GetOrdinal(CategoryNameColumn)),
                r.GetGuid(r.GetOrdinal(CategoryIdColumn))
            ))
            .Results(ct);
}
