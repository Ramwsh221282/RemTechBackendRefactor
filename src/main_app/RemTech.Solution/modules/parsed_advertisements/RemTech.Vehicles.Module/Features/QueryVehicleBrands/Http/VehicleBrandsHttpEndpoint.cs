using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.Vehicles.Module.Features.QueryVehicleBrands.Http;

public static class VehicleBrandsHttpEndpoint
{
    public static void BrandsEndpoint(this RouteGroupBuilder group) =>
        group.MapGet("kinds/{kindId:Guid}/brands", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource connectionSource,
        [FromServices] Serilog.ILogger logger,
        [FromRoute] Guid kindId,
        CancellationToken ct
    )
    {
        await using NpgsqlConnection connection = await connectionSource.OpenConnectionAsync(ct);
        try
        {
            return Results.Ok(
                await connection.Provide(
                    kindId,
                    VehicleBrandPresentationSource.CreateCommand,
                    VehicleBrandPresentationSource.CreateReader,
                    VehicleBrandPresentationSource.ProcessWithReader,
                    ct: ct
                )
            );
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance} {Error}.", nameof(VehicleBrandsHttpEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}
