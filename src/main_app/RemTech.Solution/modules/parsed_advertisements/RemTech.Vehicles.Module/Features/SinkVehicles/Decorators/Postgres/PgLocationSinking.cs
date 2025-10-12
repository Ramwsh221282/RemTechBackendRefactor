using GeoLocations.Module.Features.Querying;
using RemTech.Vehicles.Module.Features.SinkVehicles.Types;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

internal sealed class PgLocationSinking(
    IGeoLocationQueryService service,
    ITransportAdvertisementSinking sinking
) : ITransportAdvertisementSinking
{
    public async Task<Result.Pattern.Result> Sink(
        IVehicleJsonSink sink,
        CancellationToken ct = default
    )
    {
        SinkedVehicleLocation location = sink.Location();
        GeoLocationInfo persisted = await service.VectorSearch(location.Text, ct);
        return await sinking.Sink(
            new CachedVehicleJsonSink(
                sink,
                new SinkedVehicleLocation(
                    persisted.Region.Name,
                    persisted.Region.Type,
                    persisted.City.Name,
                    persisted.Region.Id
                )
            ),
            ct
        );
    }
}
