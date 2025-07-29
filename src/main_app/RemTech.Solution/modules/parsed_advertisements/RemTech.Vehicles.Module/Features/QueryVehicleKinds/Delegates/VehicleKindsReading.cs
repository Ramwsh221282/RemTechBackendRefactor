using RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehicleKinds.Delegates;

public delegate Task<IEnumerable<VehicleKindPresentation>> VehicleKindsReading(
    VehicleKindPresentationReader reader,
    CancellationToken ct = default
);
