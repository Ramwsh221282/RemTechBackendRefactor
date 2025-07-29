using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.Vehicles.Module.Features.QueryVehicleModels.Http;

public static class VehicleModelsHttpEndpoint
{
    public static void ModelsEndpoint(this RouteGroupBuilder group) =>
        group.MapGet("kinds/{kindId:Guid}/brands/{brandId:Guid}/models", Handle);

    private static async Task<IResult> Handle(
        [FromServices] PgConnectionSource connectionSource,
        [FromRoute] Guid kindId,
        [FromRoute] Guid brandId,
        CancellationToken cancellation
    )
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(cancellation);
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
}
