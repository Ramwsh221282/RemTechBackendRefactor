using RemTech.ParsedAdvertisements.Core.Types.Kinds;
using RemTech.ParsedAdvertisements.Core.Types.Kinds.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Types.Kinds.Decorators.Validation;
using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Features.SinkVehicles.Decorators.Postgres;

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
