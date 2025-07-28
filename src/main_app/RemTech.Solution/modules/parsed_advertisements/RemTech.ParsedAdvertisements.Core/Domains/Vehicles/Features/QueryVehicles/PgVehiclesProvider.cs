using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Presenting;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles;

public sealed class PgVehiclesProvider(
    PgConnectionSource connectionSource,
    Serilog.ILogger logger,
    VehiclesQueryRequest request)
{
    public async Task<IEnumerable<Presenting.VehiclePresentation>> Provide(CancellationToken ct = default)
    {
        VehiclesSqlQuery query = new VehiclesSqlQuery(logger).ApplyRequest(request);
        IEnumerable<Vehicle> vehicles = await query.Retrieve(connectionSource, ct);
        IEnumerable<Presenting.VehiclePresentation> presentations = vehicles.Select(v => new PresentingVehicle(v).Present());
        return presentations;
    }
}