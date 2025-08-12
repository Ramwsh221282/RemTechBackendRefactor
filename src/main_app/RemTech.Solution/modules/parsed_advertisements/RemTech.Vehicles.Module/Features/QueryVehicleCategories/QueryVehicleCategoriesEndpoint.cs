using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace RemTech.Vehicles.Module.Features.QueryVehicleCategories;

internal static class QueryVehicleCategoriesEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("categories", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        try
        {
            QueryVehicleCategoriesCommand command = new();
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            QueryVehicleCategoriesHandler handler = new(dataSource);
            IEnumerable<VehicleCategoriesResult> results = await handler.Handle(command, ct);
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(QueryVehicleCategoriesEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
