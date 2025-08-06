using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehicles.Arguments;
using RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Http;

public static class VehiclesCatalogueHttpEndpoint
{
    public static void CatalogueEndpoint(this RouteGroupBuilder group) =>
        group.MapPost(
            "kinds/{kindId:Guid}/brands/{brandId:Guid}/models/{modelId:Guid}/catalogue",
            Handle
        );

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource connectionSource,
        [FromServices] Serilog.ILogger logger,
        [FromBody] VehiclesQueryRequest request,
        [FromRoute] Guid kindId,
        [FromRoute] Guid brandId,
        [FromRoute] Guid modelId,
        CancellationToken ct
    )
    {
        request = request with
        {
            BrandId = new VehicleBrandIdQueryFilterArgument(brandId),
            KindId = new VehicleKindIdQueryFilterArgument(kindId),
            ModelId = new VehicleModelIdQueryFilterArgument(modelId),
        };
        await using NpgsqlConnection connection = await connectionSource.OpenConnectionAsync(ct);
        IEnumerable<VehiclePresentation> vehicles = await new PgVehiclesProvider(
            connection,
            logger
        ).Provide(request, ct);
        return Results.Ok(vehicles);
    }
}
