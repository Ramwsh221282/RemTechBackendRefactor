using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;
using RemTech.Vehicles.Module.Types.Brands;
using RemTech.Vehicles.Module.Types.Brands.Decorators.Postgres;
using RemTech.Vehicles.Module.Types.Brands.Decorators.Validation;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

public sealed class PgVehicleBrandSinking(
    PgConnectionSource connection,
    ITransportAdvertisementSinking sinking
) : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        VehicleBrand brand = sink.Brand();
        VehicleBrand valid = new ValidVehicleBrand(brand);
        VehicleBrand saved = await new PgVarietVehicleBrand(connection, valid).SaveAsync(ct);
        return await sinking.Sink(new CachedVehicleJsonSink(sink, saved), ct);
    }
}
