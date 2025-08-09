using Npgsql;
using RemTech.Vehicles.Module.Database.Embeddings;
using RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;

namespace RemTech.Vehicles.Module.Features.QueryVehicles;

public sealed class PgVehiclesProvider(
    NpgsqlConnection connection,
    Serilog.ILogger logger,
    IEmbeddingGenerator generator
)
{
    public async Task<IEnumerable<VehiclePresentation>> Provide(
        VehiclesQueryRequest request,
        CancellationToken ct = default
    )
    {
        VehiclesSqlQuery query = new VehiclesSqlQuery(logger, generator).ApplyRequest(request);
        return await query.Retrieve(connection, ct);
    }
}
