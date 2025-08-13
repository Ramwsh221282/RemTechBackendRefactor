using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Categories.Module.Features.QueryCategoriesAmount;

public static class QueryCategoriesAmountEndpoint
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
            QueryCategoriesAmountCommand command = new(text);
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            QueryCategoriesAmountHandler handler = new(connection, generator);
            long result = await handler.Handle(command, ct);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(QueryCategoriesAmountEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
