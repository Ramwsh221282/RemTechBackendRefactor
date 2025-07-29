using RemTech.ParsedAdvertisements.Core.Types.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations.Decorators.Validation;
using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Features.SinkVehicles.Decorators.Postgres;

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
