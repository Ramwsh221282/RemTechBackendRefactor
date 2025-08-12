using Models.Module.Public;
using RemTech.Core.Shared.Result;
using RemTech.Vehicles.Module.Features.SinkVehicles.Types;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

internal sealed class PgModelSinking(
    IModelPublicApi modelApi,
    ITransportAdvertisementSinking sinking
) : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        SinkedVehicleModel model = sink.Model();
        SinkedVehicleModel persisted = await ModelResponse.MapTo(
            r => new SinkedVehicleModel(r.Name, r.Id),
            () => modelApi.Get(model.Name, ct)
        );
        return await sinking.Sink(new CachedVehicleJsonSink(sink, persisted), ct);
    }
}
