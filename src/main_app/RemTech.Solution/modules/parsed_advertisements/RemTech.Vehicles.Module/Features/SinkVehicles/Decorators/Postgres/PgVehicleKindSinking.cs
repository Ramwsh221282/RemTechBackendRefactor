using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;
using RemTech.Vehicles.Module.Types.Kinds;
using RemTech.Vehicles.Module.Types.Kinds.Decorators.Postgres;
using RemTech.Vehicles.Module.Types.Kinds.Decorators.Validation;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

public sealed class PgVehicleKindSinking(
    PgConnectionSource connection,
    ITransportAdvertisementSinking sinking
) : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        VehicleKind kind = sink.Kind();
        VehicleKind saved = await new PgVarietVehicleKind(
            connection,
            new ValidVehicleKind(kind)
        ).SaveAsync(ct);
        return await sinking.Sink(new CachedVehicleJsonSink(sink, saved), ct);
    }
}
