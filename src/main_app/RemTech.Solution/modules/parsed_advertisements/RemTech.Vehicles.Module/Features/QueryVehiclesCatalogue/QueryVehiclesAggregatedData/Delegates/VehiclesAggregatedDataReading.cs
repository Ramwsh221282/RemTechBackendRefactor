using System.Data.Common;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData.Delegates;

public delegate Task<VehiclesAggregatedDataPresentation> VehiclesAggregatedDataReading(
    DbDataReader reader,
    CancellationToken ct = default
);
