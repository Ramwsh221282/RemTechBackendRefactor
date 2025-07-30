using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Postgres.Adapter.Library;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.Http;

public static class VehiclesCatalogueHttpEndpoint
{
    public static void CatalogueEndpoint(this RouteGroupBuilder group) =>
        group.MapPost(
            "kinds/{kindId:Guid}/brands/{brandId:Guid}/models/{modelId:Guid}/catalogue",
            Handle
        );

    private static async Task<IResult> Handle(
        [FromServices] PgConnectionSource connectionSource,
        [FromServices] Serilog.ILogger logger,
        [FromBody] VehiclesQueryRequest request,
        [FromRoute] Guid kindId,
        [FromRoute] Guid brandId,
        [FromRoute] Guid modelId,
        CancellationToken ct
    )
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        request = request with
        {
            BrandId = new VehicleBrandIdQueryFilterArgument(brandId),
            KindId = new VehicleKindIdQueryFilterArgument(kindId),
            ModelId = new VehicleModelIdQueryFilterArgument(modelId),
        };
        return Results.Ok(
            await VehiclesCatalogueProvider.VehiclesCatalogue()(
                connection.VehiclesOfCatalogue(logger, request, ct),
                connection.CharacteristicsOfCatalogue(request, ct),
                connection.AggregatedDataOfCatalogue(request, ct),
                connection.GeoLocationsOfCatalogue(request, ct)
            )
        );
    }
}
