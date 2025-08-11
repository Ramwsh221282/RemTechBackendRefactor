using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;
using Shared.Infrastructure.Module.Postgres.Embeddings;

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
