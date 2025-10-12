using Brands.Module.Public;
using RemTech.Vehicles.Module.Features.SinkVehicles.Types;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

internal sealed class BrandSpecifying(
    IBrandsPublicApi brandsApi,
    ITransportAdvertisementSinking inner
) : ITransportAdvertisementSinking
{
    public async Task<Result.Pattern.Result> Sink(
        IVehicleJsonSink sink,
        CancellationToken ct = default
    )
    {
        SinkedVehicleBrand brand = sink.Brand();
        SinkedVehicleBrand persisted = await BrandResponse.MapTo(
            r => new SinkedVehicleBrand(r.Name, r.Id),
            () => brandsApi.GetBrand(brand.Name, ct)
        );
        return await inner.Sink(new CachedVehicleJsonSink(sink, persisted), ct);
    }
}
