using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;
using RemTech.Vehicles.Module.Types.GeoLocations;
using RemTech.Vehicles.Module.Types.GeoLocations.Decorators.Postgres;
using RemTech.Vehicles.Module.Types.GeoLocations.Decorators.Validation;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

public sealed class PgLocationSinking(
    PgConnectionSource connection,
    ITransportAdvertisementSinking sinking
) : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        GeoLocation location = sink.Location();
        GeoLocation saved = await new PgGeoLocation(
            connection,
            new ValidGeoLocation(location)
        ).SaveAsync(ct);
        return await sinking.Sink(new CachedVehicleJsonSink(sink, saved), ct);
    }
}
