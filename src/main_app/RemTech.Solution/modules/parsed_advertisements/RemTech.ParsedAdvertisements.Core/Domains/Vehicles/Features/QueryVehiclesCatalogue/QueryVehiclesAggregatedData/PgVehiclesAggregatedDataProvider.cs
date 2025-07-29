using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData.Types;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData;

public sealed class PgVehiclesAggregatedDataProvider(NpgsqlConnection connection)
{
    public async Task<VehiclesAggregatedDataPresentation> Provide(
        VehiclesQueryRequest request,
        CancellationToken ct
    ) =>
        await VehiclesAggregatedDataPresentation.Read(
            new VehiclesAggregatedDataSqlQuery().AcceptRequest(request),
            connection,
            ct
        );
}
