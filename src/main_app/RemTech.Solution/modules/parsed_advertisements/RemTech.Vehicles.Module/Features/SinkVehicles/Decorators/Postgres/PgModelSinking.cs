using Npgsql;
using RemTech.Result.Library;
using RemTech.Vehicles.Module.Database.Embeddings;
using RemTech.Vehicles.Module.Types.Models;
using RemTech.Vehicles.Module.Types.Models.Decorators.Validation;
using RemTech.Vehicles.Module.Types.Models.Storage;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

public sealed class PgModelSinking(
    NpgsqlDataSource connection,
    ITransportAdvertisementSinking sinking,
    IEmbeddingGenerator generator
) : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        VehicleModel model = sink.Model();
        VehicleModel valid = new ValidVehicleModel(model);
        VehicleModel saved = await new VarietVehicleModelsStorage()
            .With(new RawByNameVehicleModelsStorage(connection))
            .With(new VectorVehicleModelsStorage(connection, generator))
            .With(new TsQueryVehicleModelsStorage(connection))
            .With(new PgTgrmVehicleModelsStorage(connection))
            .With(new NewVehicleModelsStorage(connection, generator))
            .Store(valid);
        return await sinking.Sink(new CachedVehicleJsonSink(sink, saved), ct);
    }
}
