using RemTech.Vehicles.Module.Features.QueryVehicleBrands.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehicleBrands.Delegates;

public delegate Task<IEnumerable<VehicleBrandPresentation>> QueriedVehicleBrands(
    VehicleBrandPresentationReader reader,
    CancellationToken ct = default
);
