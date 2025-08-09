using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace RemTech.Vehicles.Module.Features.QueryVehicleModels.Http;

public static class VehicleModelsHttpEndpoint
{
    public static void ModelsEndpoint(this RouteGroupBuilder group) =>
        group.MapGet("kinds/{kindId:Guid}/brands/{brandId:Guid}/models", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource connectionSource,
        [FromServices] Serilog.ILogger logger,
        [FromRoute] Guid kindId,
        [FromRoute] Guid brandId,
        CancellationToken cancellation
    )
    {
        try
        {
            await using NpgsqlConnection connection = await connectionSource.OpenConnectionAsync(
                cancellation
            );
            return Results.Ok(
                await connection.Provide(
                    kindId,
                    brandId,
                    VehicleModelPresentationSource.VehicleModelsCommandSource,
                    VehicleModelPresentationSource.VehicleModelsReaderSource,
                    VehicleModelPresentationSource.VehicleModelsReadingSource,
                    cancellation
                )
            );
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Error}.", nameof(VehicleModelsHttpEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
