using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData;

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
