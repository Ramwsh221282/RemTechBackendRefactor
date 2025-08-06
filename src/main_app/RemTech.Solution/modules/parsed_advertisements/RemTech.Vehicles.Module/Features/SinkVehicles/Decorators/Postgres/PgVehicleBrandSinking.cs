using Npgsql;
using RemTech.Result.Library;
using RemTech.Vehicles.Module.Types.Brands;
using RemTech.Vehicles.Module.Types.Brands.Decorators.Validation;
using RemTech.Vehicles.Module.Types.Brands.Storage;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

public sealed class PgVehicleBrandSinking(
    NpgsqlDataSource connection,
    ITransportAdvertisementSinking sinking
) : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        VehicleBrand brand = sink.Brand();
        VehicleBrand valid = new ValidVehicleBrand(brand);
        VehicleBrand saved = await new VarietVehicleBrandsStorage()
            .With(new RawByNameVehicleBrandsStorage(connection))
            .With(new TsQueryVehicleBrandsStorage(connection))
            .With(new PgTgrmVehicleBrandsStorage(connection))
            .With(new NewVehicleBrandsStorage(connection))
            .Store(valid);
        return await sinking.Sink(new CachedVehicleJsonSink(sink, saved), ct);
    }
}
