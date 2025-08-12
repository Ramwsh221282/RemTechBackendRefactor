using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehicles.Arguments;
using RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Http;

public static class VehiclesCatalogueHttpEndpoint
{
    public static void CatalogueEndpoint(this RouteGroupBuilder group) =>
        group.MapGet(string.Empty, Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource connectionSource,
        [FromServices] Serilog.ILogger logger,
        [FromServices] IEmbeddingGenerator generator,
        [FromQuery] int page,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? brandId,
        [FromQuery] Guid? modelId,
        [FromQuery] Guid? locationId,
        [FromQuery] string? sort,
        [FromQuery] string? text,
        [FromQuery] long? priceMin,
        [FromQuery] long? priceMax,
        CancellationToken ct
    )
    {
        try
        {
            if (page <= 0)
                return Results.Ok(Enumerable.Empty<VehiclePresentation>());
            VehiclesQueryRequest request = new VehiclesQueryRequest()
            {
                Pagination = new VehiclePaginationQueryFilterArgument(page),
                BrandId =
                    brandId == null ? null : new VehicleBrandIdQueryFilterArgument(brandId.Value),
                KindId =
                    categoryId == null
                        ? null
                        : new VehicleKindIdQueryFilterArgument(categoryId.Value),
                ModelId =
                    modelId == null ? null : new VehicleModelIdQueryFilterArgument(modelId.Value),
                RegionId =
                    locationId == null
                        ? null
                        : new VehicleRegionIdQueryFilterArgument(locationId.Value),
                Text = string.IsNullOrWhiteSpace(text)
                    ? null
                    : new VehicleTextSearchQueryFilterArgument(text),
                SortOrder = string.IsNullOrWhiteSpace(sort)
                    ? null
                    : new VehicleSortOrderQueryFilterArgument(sort),
                Price = new VehiclePriceQueryFilterArgument(null, priceMin, priceMax),
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
