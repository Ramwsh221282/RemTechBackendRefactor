using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData.Types;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData.Delegates;

public delegate Task<VehiclesAggregatedDataPresentation> VehiclesAggregatedDataReading(
    DbDataReader reader,
    CancellationToken ct = default
);
