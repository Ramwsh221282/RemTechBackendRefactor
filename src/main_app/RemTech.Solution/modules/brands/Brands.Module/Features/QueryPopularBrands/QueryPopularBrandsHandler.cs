using Npgsql;
using RemTech.Core.Shared.Cqrs;
using Shared.Infrastructure.Module.Postgres.PgCommandsBetter;

namespace Brands.Module.Features.QueryPopularBrands;

internal sealed class QueryPopularBrandsHandler(NpgsqlConnection connection)
    : ICommandHandler<QueryPopularBrandsCommand, IEnumerable<PopularBrandsResponse>>
{
    private const string Sql = """
        SELECT b.name as brand_name, b.id as brand_id, COUNT(p.id) as vehicles_amount FROM brands_module.brands b
        LEFT JOIN parsed_advertisements_module.parsed_vehicles p ON b.id = p.brand_id                             
        GROUP BY b.name, b.id HAVING(COUNT(p.id) > 0)
        ORDER BY vehicles_amount DESC
        LIMIT 6;
        """;
    private const string BrandNameColumn = "brand_name";
    private const string BrandIdColumn = "brand_id";

    public async Task<IEnumerable<PopularBrandsResponse>> Handle(
        QueryPopularBrandsCommand command,
        CancellationToken ct = default
    ) =>
        await connection
            .Command()
            .WithQueryString(Sql)
            .ClosureReader(ct)
            .AsBlockOf<PopularBrandsResponse>()
            .Map(r => new PopularBrandsResponse(
                r.GetString(r.GetOrdinal(BrandNameColumn)),
                r.GetGuid(r.GetOrdinal(BrandIdColumn))
            ))
            .Results(ct);
}
