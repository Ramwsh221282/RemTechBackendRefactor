using Brands.Module.Responses;
using Dapper;
using RemTech.Core.Shared.Cqrs;
using Shared.Infrastructure.Module.Postgres;

namespace Brands.Module.Features.QueryPopularBrands;

internal sealed class QueryPopularBrandsHandler(PostgresDatabase database)
    : ICommandHandler<QueryPopularBrandsCommand, IEnumerable<BrandDto>>
{
    private const string Sql = """
        SELECT 
            b.name as name, 
            b.id as id, 
            COUNT(p.id) as items_count 
        FROM brands_module.brands b
        LEFT JOIN parsed_advertisements_module.parsed_vehicles p ON b.id = p.brand_id                             
        GROUP BY b.name, b.id HAVING(COUNT(p.id) > 0)
        ORDER BY items_count DESC
        LIMIT 6;
        """;

    public async Task<IEnumerable<BrandDto>> Handle(
        QueryPopularBrandsCommand command,
        CancellationToken ct = default
    )
    {
        var sqlCommand = new CommandDefinition(Sql, cancellationToken: ct);
        using var connection = await database.ProvideConnection(ct);
        return await connection.QueryAsync<BrandDto>(sqlCommand);
    }
}
