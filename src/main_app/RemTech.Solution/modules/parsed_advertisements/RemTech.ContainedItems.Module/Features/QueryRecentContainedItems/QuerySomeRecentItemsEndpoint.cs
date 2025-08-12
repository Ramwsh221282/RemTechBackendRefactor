using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace RemTech.ContainedItems.Module.Features.QueryRecentContainedItems;

internal static class QuerySomeRecentItemsEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.Map("recent", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] int page,
        CancellationToken ct
    )
    {
        try
        {
            QueryRecentContainedItemsCommand command = new(page);
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            QueryRecentContainedItemsHandler handler = new(connection);
            IEnumerable<SomeRecentItem> items = await handler.Handle(command, ct);
            return Results.Ok(items);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(QuerySomeRecentItemsEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на уровне приложения" });
        }
    }
}
