using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Brands.Module.Features.QueryBrandsAmount;

internal static class QueryBrandsAmountEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("count", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromServices] IEmbeddingGenerator generator,
        [FromQuery] string? text,
        CancellationToken ct
    )
    {
        try
        {
            QueryBrandsAmountCommand command = new(text);
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            QueryBrandsAmountHandler handler = new(connection, generator);
            long count = await handler.Handle(command, ct);
            return Results.Ok(count);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(QueryBrandsAmountEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на уровне приложения." });
        }
    }
}
