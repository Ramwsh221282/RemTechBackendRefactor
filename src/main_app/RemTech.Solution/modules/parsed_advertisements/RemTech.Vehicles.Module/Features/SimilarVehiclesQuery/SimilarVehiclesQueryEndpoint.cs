using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Vehicles.Module.Features.SimilarVehiclesQuery;

public static class SimilarVehiclesQueryEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("similar", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] IEmbeddingGenerator generator,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] string text,
        [FromQuery] string vehicleId,
        CancellationToken ct
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(text))
                return Results.Ok(Enumerable.Empty<VehiclePresentation>());
            SimilarVehiclesQuery query = new SimilarVehiclesQuery();
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            IEnumerable<VehiclePresentation> results = await query.Query(
                vehicleId,
                text,
                connection,
                generator,
                ct
            );
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance} {Ex}.", nameof(SimilarVehiclesQueryEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}
