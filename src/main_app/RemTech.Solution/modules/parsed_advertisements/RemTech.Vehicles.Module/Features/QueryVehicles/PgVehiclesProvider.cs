using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Vehicles.Module.Features.QueryVehicles;

public sealed class PgVehiclesProvider(NpgsqlConnection connection, IEmbeddingGenerator generator)
{
    public async Task<IEnumerable<VehiclePresentation>> Provide(
        VehiclesQueryRequest request,
        CancellationToken ct = default
    )
    {
        VehiclesSqlQuery query = new VehiclesSqlQuery(generator).ApplyRequest(request);
        return await query.Retrieve(connection, ct);
    }
}
