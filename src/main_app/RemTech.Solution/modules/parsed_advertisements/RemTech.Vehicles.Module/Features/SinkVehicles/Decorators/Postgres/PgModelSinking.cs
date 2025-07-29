using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;
using RemTech.Vehicles.Module.Types.Models;
using RemTech.Vehicles.Module.Types.Models.Decorators.Postgres;
using RemTech.Vehicles.Module.Types.Models.Decorators.Validation;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

public sealed class PgModelSinking(
    PgConnectionSource connection,
    ITransportAdvertisementSinking sinking
) : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        VehicleModel model = sink.Model();
        VehicleModel saved = await new PgVarietVehicleModel(
            connection,
            new ValidVehicleModel(model)
        ).SaveAsync(ct);
        return await sinking.Sink(new CachedVehicleJsonSink(sink, saved), ct);
    }
}
