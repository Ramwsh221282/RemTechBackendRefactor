using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Brands.Module.Features.QueryBrands;

internal static class QueryBrandsEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("all", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromServices] IEmbeddingGenerator generator,
        [FromQuery] int page,
        [FromQuery] string? text,
        CancellationToken ct
    )
    {
        try
        {
            QueryBrandsCommand command = new(page, text);
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            QueryBrandsHandler handler = new QueryBrandsHandler(connection, generator);
            IEnumerable<QueryBrandResult> results = await handler.Handle(command, ct);
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance} {Ex}.", nameof(QueryBrandsEndpoint), ex);
            return Results.InternalServerError(new { message = "Ошибка на уровне приложения" });
        }
    }
}
