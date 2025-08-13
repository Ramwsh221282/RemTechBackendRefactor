using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace RemTech.Vehicles.Module.Features.QueryVehicleModels;

public static class QueryVehicleModelsEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("models", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        try
        {
            QueryVehicleModelsCommand command = new();
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            QueryVehicleModelsHandler handler = new(dataSource);
            IEnumerable<QueryVehicleModelsResult> results = await handler.Handle(command, ct);
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(QueryVehicleModelsEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
