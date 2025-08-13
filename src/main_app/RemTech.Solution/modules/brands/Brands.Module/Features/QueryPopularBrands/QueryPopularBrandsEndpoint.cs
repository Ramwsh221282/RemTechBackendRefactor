using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace Brands.Module.Features.QueryPopularBrands;

public static class QueryPopularBrandsEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("popular", Handle);

    private static async Task<IResult> Handle(
        [FromServices] Serilog.ILogger logger,
        [FromServices] NpgsqlDataSource dataSource,
        CancellationToken ct
    )
    {
        try
        {
            QueryPopularBrandsCommand command = new();
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            QueryPopularBrandsHandler handler = new(connection);
            IEnumerable<PopularBrandsResponse> results = await handler.Handle(command, ct);
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(QueryPopularBrandsEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
