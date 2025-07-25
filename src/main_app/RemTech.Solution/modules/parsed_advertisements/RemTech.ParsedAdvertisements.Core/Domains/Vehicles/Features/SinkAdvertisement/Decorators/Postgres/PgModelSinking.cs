using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Adapters.Storage.Postgres;
using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkAdvertisement.Decorators.Postgres;

public sealed class PgModelSinking(PgConnectionSource connection, ITransportAdvertisementSinking sinking)
    : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        VehicleModel model = sink.Model();
        VehicleModel saved = await new PgVarietVehicleModelsStorage()
            .With(new PgVehicleModelsStorage(connection))
            .With(new PgDuplicateResolvingVehicleModelsStorage(connection))
            .Get(model, ct);
        return await sinking.Sink(new CachedVehicleJsonSink(sink, saved), ct);
    }
}