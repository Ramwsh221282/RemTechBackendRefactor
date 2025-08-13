using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace RemTech.ContainedItems.Module.Features.QueryRecentContainedItemsCount;

public static class QueryRecentContainedItemsEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("recent/count", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        try
        {
            QueryRecentContainedItemsCountCommand command = new();
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            QueryRecentContainedItemsCountHandler handler = new(connection);
            long amount = await handler.Handle(command, ct);
            return Results.Ok(amount);
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Entrance}. {Ex}.",
                nameof(QueryRecentContainedItemsEndpoint),
                ex.Message
            );
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}
