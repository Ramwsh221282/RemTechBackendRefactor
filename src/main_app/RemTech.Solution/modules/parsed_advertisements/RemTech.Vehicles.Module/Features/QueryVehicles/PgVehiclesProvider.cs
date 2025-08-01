using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;

namespace RemTech.Vehicles.Module.Features.QueryVehicles;

public sealed class PgVehiclesProvider(NpgsqlConnection connection, Serilog.ILogger logger)
{
    public async Task<IEnumerable<VehiclePresentation>> Provide(
        VehiclesQueryRequest request,
        CancellationToken ct = default
    )
    {
        VehiclesSqlQuery query = new VehiclesSqlQuery(logger).ApplyRequest(request);
        return await query.Retrieve(connection, ct);
    }
}
