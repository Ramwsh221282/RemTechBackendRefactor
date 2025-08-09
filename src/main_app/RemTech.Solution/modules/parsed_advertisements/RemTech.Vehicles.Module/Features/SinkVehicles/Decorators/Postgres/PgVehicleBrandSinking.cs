using Npgsql;
using RemTech.Result.Library;
using RemTech.Vehicles.Module.Database.Embeddings;
using RemTech.Vehicles.Module.Types.Brands;
using RemTech.Vehicles.Module.Types.Brands.Decorators.Validation;
using RemTech.Vehicles.Module.Types.Brands.Storage;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

public sealed class PgVehicleBrandSinking(
    NpgsqlDataSource connection,
    ITransportAdvertisementSinking sinking,
    IEmbeddingGenerator generator
) : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        VehicleBrand brand = sink.Brand();
        VehicleBrand valid = new ValidVehicleBrand(brand);
        VehicleBrand saved = await new VarietVehicleBrandsStorage()
            .With(new RawByNameVehicleBrandsStorage(connection))
            .With(new VectorBrandsStorage(connection, generator))
            .With(new TsQueryVehicleBrandsStorage(connection))
            .With(new PgTgrmVehicleBrandsStorage(connection))
            .With(new NewVehicleBrandsStorage(connection, generator))
            .Store(valid);
        return await sinking.Sink(new CachedVehicleJsonSink(sink, saved), ct);
    }
}
