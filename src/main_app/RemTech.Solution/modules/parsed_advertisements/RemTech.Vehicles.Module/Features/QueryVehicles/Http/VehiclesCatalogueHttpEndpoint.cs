using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Vehicles.Module.Database.Embeddings;
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
        [FromServices] IEmbeddingGenerator generator,
        [FromBody] VehiclesQueryRequest request,
        [FromRoute] Guid kindId,
        [FromRoute] Guid brandId,
        [FromRoute] Guid modelId,
        CancellationToken ct
    )
    {
        try
        {
            request = request with
            {
                BrandId = new VehicleBrandIdQueryFilterArgument(brandId),
                KindId = new VehicleKindIdQueryFilterArgument(kindId),
                ModelId = new VehicleModelIdQueryFilterArgument(modelId),
            };
            await using NpgsqlConnection connection = await connectionSource.OpenConnectionAsync(
                ct
            );
            Stopwatch sw = new Stopwatch();
            sw.Start();
            IEnumerable<VehiclePresentation> vehicles = await new PgVehiclesProvider(
                connection,
                logger,
                generator
            ).Provide(request, ct);
            sw.Stop();
            logger.Information("Queried vehicles. Elapsed: {Elapsed}", sw.ElapsedMilliseconds);
            return Results.Ok(vehicles);
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Entrance} exception: {Ex}.",
                nameof(VehiclesCatalogueHttpEndpoint),
                ex.Message
            );
            return Results.InternalServerError(Enumerable.Empty<VehiclePresentation>());
        }
    }
}
