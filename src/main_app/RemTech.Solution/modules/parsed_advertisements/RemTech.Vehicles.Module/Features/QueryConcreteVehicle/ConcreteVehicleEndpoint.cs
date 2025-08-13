using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;

namespace RemTech.Vehicles.Module.Features.QueryConcreteVehicle;

public static class ConcreteVehicleEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("item", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] string vehicleId,
        CancellationToken ct
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(vehicleId))
                return Results.NotFound();
            ConcreteVehicleQuery query = new();
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            VehiclePresentation? vehicle = await query.Query(vehicleId, connection, ct);
            return vehicle == null ? Results.NotFound() : Results.Ok(vehicle);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(ConcreteVehicleEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}
