using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace RemTech.ContainedItems.Module.Features.GetContainedVehiclesAmount;

internal static class GetContainedItemsByTypeEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("count", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        try
        {
            GetContainedItemsByTypeCommand command = new();
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            GetContainedItemsByTypeHandler handler = new GetContainedItemsByTypeHandler(connection);
            IEnumerable<GetContainedItemsByTypeResponse> response = await handler.Handle(
                command,
                ct
            );
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(GetContainedItemsByTypeEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
