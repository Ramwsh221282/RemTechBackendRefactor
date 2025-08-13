using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace RemTech.Vehicles.Module.Features.QueryVehicleRegions;

public static class QueryVehicleRegionsEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("regions", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        try
        {
            QueryVehicleRegionsCommand command = new();
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            QueryVehicleRegionsHandler handler = new QueryVehicleRegionsHandler(dataSource);
            IEnumerable<QueryVehicleRegionsResult> results = await handler.Handle(command, ct);
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(QueryVehicleRegionsEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
