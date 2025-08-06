using Npgsql;
using RemTech.Result.Library;
using RemTech.Vehicles.Module.Types.Kinds;
using RemTech.Vehicles.Module.Types.Kinds.Decorators.Validation;
using RemTech.Vehicles.Module.Types.Kinds.Storage;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

public sealed class PgVehicleKindSinking(
    NpgsqlDataSource connection,
    ITransportAdvertisementSinking sinking
) : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        VehicleKind kind = sink.Kind();
        ValidVehicleKind valid = new ValidVehicleKind(kind);
        VehicleKind saved = await new VarietVehicleKindsStorage()
            .With(new RawByNameVehicleKindsStorage(connection))
            .With(new TsQueryVehicleKindsStorge(connection))
            .With(new PgTgrmVehicleKindsStorage(connection))
            .With(new NewVehicleKindsStorage(connection))
            .Store(valid);
        return await sinking.Sink(new CachedVehicleJsonSink(sink, saved), ct);
    }
}
