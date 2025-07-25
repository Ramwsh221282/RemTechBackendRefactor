using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Adapters.Storage.Postgres;
using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkAdvertisement.Decorators.Postgres;

public sealed class PgVehicleBrandSinking(PgConnectionSource connection, ITransportAdvertisementSinking sinking)
    : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        VehicleBrand brand = sink.Brand();
        VehicleBrand saved = await new PgVarietVehicleBrandsStorage()
            .With(new PgVehicleBrandsStore(connection))
            .With(new PgDuplicateResolvingVehicleBrandsStore(connection))
            .Get(brand, ct);
        return await sinking.Sink(new CachedVehicleJsonSink(sink, saved), ct);
    }
}