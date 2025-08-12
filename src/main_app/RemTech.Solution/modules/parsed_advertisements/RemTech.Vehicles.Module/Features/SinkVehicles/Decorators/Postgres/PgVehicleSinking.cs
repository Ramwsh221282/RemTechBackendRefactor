using Npgsql;
using RemTech.Core.Shared.Result;
using RemTech.Vehicles.Module.Features.SinkVehicles.Types;
using RemTech.Vehicles.Module.Types.Transport;
using RemTech.Vehicles.Module.Types.Transport.Decorators;
using RemTech.Vehicles.Module.Types.Transport.Decorators.Postgres;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

internal sealed class PgVehicleSinking(
    NpgsqlDataSource connection,
    IEmbeddingGenerator generator,
    ITransportAdvertisementSinking sinking
) : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        Vehicle vehicle = sink.Vehicle();
        SinkedVehicleCategory category = sink.Category();
        SinkedVehicleBrand brand = sink.Brand();
        SinkedVehicleModel model = sink.Model();
        SinkedVehicleLocation location = sink.Location();
        Vehicle specified = vehicle.Accept(category).Accept(brand).Accept(model).Accept(location);
        Vehicle valid = new ValidVehicle(specified);
        await new PgTransactionalVehicle(connection, generator, valid).SaveAsync(ct);
        return await sinking.Sink(sink, ct);
    }
}
