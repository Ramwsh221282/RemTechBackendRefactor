using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Decorators.Validation;
using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkVehicles.Decorators.Postgres;

public sealed class PgModelSinking(PgConnectionSource connection, ITransportAdvertisementSinking sinking)
    : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        VehicleModel model = sink.Model();
        VehicleModel saved = await new PgVarietVehicleModel(connection,
                new ValidVehicleModel(model))
            .SaveAsync(ct);
        return await sinking.Sink(new CachedVehicleJsonSink(sink, saved), ct);
    }
}