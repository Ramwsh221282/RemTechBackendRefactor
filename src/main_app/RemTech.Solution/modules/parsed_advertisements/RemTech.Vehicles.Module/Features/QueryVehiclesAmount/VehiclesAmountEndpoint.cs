using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehicles.Arguments;
using RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesAmount;

public sealed class VehiclesAmountEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("count", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource datasource,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? brandId,
        [FromQuery] Guid? modelId,
        [FromQuery] Guid? locationId,
        [FromQuery] string? text,
        [FromQuery] long? priceMin,
        [FromQuery] long? priceMax,
        CancellationToken ct
    )
    {
        try
        {
            VehiclesAmountRequest request = new()
            {
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
                Price = new VehiclePriceQueryFilterArgument(null, priceMin, priceMax),
            };

            await using NpgsqlConnection connection = await datasource.OpenConnectionAsync(ct);
            VehiclesAmountSqlQuery query = new VehiclesAmountSqlQuery().ApplyRequest(request);
            long amount = await query.Retrieve(connection, ct);
            return Results.Ok(amount);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance} exception: {Ex}.", nameof(VehiclesAmountEndpoint), ex.Message);
            return Results.InternalServerError(Enumerable.Empty<VehiclePresentation>());
        }
    }
}
