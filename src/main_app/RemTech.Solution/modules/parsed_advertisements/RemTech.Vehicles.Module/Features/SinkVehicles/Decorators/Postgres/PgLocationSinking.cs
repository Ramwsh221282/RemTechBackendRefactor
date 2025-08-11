using Npgsql;
using RemTech.Result.Library;
using RemTech.Vehicles.Module.Types.GeoLocations;
using RemTech.Vehicles.Module.Types.GeoLocations.Decorators.Validation;
using RemTech.Vehicles.Module.Types.GeoLocations.Storage;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

public sealed class PgLocationSinking(
    NpgsqlDataSource connection,
    ITransportAdvertisementSinking sinking,
    IEmbeddingGenerator generator
) : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        GeoLocation location = sink.Location();
        ValidGeoLocation valid = new(location);
        GeoLocation saved = await new VarietGeoLocationsStorage()
            .With(new RawByNameGeoLocationsStorage(connection))
            .With(new VectorSearchGeoLocationStorage(connection, generator))
            .With(new TsQueryGeoLocationsStorage(connection))
            .With(new PgTgrmGeoLocationsStorage(connection))
            .With(new NewGeoLocationsStorage(connection, generator))
            .Save(valid);
        return await sinking.Sink(new CachedVehicleJsonSink(sink, saved), ct);
    }
}
