using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace Categories.Module.Features.QueryPopularCategories;

internal static class QueryPopularCategoriesEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("popular", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        try
        {
            PopularCategoriesCommand command = new();
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            QueryPopularCategoriesHandler handler = new(connection);
            IEnumerable<PopularCategoriesResponse> results = await handler.Handle(command, ct);
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(QueryPopularCategoriesEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на уровне приложения" });
        }
    }
}
