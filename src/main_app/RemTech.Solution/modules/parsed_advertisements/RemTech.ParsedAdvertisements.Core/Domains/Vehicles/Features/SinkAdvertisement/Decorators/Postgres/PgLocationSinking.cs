using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Adapters.Storage.Postgres;
using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkAdvertisement.Decorators.Postgres;

public sealed class PgLocationSinking(PgConnectionSource connection, ITransportAdvertisementSinking sinking)
    : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        GeoLocation location = sink.Location();
        GeoLocation saved = await new PgVarietVehicleGeosStorage()
            .With(new PgVehicleGeosStorage(connection))
            .Get(location, ct);
        return  await sinking.Sink(new CachedVehicleJsonSink(sink, saved), ct);
    }
}