using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData.Types;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData.Delegates;

public delegate Task<VehiclesAggregatedDataPresentation> VehiclesAggregatedDataReading(
    DbDataReader reader,
    CancellationToken ct = default
);
