using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;
using RemTech.Vehicles.Module.Types.Transport;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles;

public sealed class PgVehiclesProvider(NpgsqlConnection connection, Serilog.ILogger logger)
{
    public async Task<IEnumerable<VehiclePresentation>> Provide(
        VehiclesQueryRequest request,
        CancellationToken ct = default
    )
    {
        VehiclesSqlQuery query = new VehiclesSqlQuery(logger).ApplyRequest(request);
        IEnumerable<Vehicle> vehicles = await query.Retrieve(connection, ct);
        IEnumerable<VehiclePresentation> presentations = vehicles.Select(v =>
            new PresentingVehicle(v).Present()
        );
        return presentations;
    }
}
