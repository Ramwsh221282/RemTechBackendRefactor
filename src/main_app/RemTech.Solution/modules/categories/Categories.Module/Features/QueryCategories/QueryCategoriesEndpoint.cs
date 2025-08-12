using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Categories.Module.Features.QueryCategories;

internal static class QueryCategoriesEndpoint
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
            QueryCategoriesCommand command = new(page, text);
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            QueryCategoriesHandler handler = new(connection, generator);
            IEnumerable<QueryCategoriesResult> results = await handler.Handle(command, ct);
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(QueryCategoriesEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на уровне приложения" });
        }
    }
}
