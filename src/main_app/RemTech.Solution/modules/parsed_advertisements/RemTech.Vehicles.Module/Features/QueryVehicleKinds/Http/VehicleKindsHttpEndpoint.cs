using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehicleKinds.Http;

public static class VehicleKindsHttpEndpoint
{
    public static void KindsEndpoint(this RouteGroupBuilder builder) =>
        builder.MapGet("kinds", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource connectionSource,
        [FromServices] Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        try
        {
            await using NpgsqlConnection connection = await connectionSource.OpenConnectionAsync(
                ct
            );
            IEnumerable<VehicleKindPresentation> kinds = await connection.Provide(
                VehicleKindsPresentationSource.VehicleKindsCommand,
                VehicleKindsPresentationSource.VehicleKindsReader,
                VehicleKindsPresentationSource.VehicleKindsReading,
                ct
            );
            return Results.Ok(kinds);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Error}.", nameof(VehicleKindsHttpEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}
