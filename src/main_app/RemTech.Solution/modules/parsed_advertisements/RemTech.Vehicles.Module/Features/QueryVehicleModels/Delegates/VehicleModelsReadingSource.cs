using RemTech.Vehicles.Module.Features.QueryVehicleModels.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehicleModels.Delegates;

public delegate Task<IEnumerable<VehicleModelPresentation>> VehicleModelsReadingSource(
    VehicleModelPresentationReader reader,
    CancellationToken ct = default
);
