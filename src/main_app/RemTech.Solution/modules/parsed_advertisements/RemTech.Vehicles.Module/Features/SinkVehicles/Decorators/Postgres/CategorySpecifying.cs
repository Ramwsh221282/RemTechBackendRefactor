using Categories.Module.Public;
using RemTech.Core.Shared.Result;
using RemTech.Vehicles.Module.Features.SinkVehicles.Types;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

internal sealed class CategorySpecifying(
    ICategoryPublicApi categoryApi,
    ITransportAdvertisementSinking sinking
) : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        SinkedVehicleCategory sinked = sink.Category();
        SinkedVehicleCategory persisted = await CategoryResponse.MapTo(
            c => new SinkedVehicleCategory(c.Name, c.Id),
            () => categoryApi.GetCategory(sinked.Name, ct)
        );
        return await sinking.Sink(new CachedVehicleJsonSink(sink, persisted), ct);
    }
}
