namespace RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types;

public sealed record VehicleKindPresentation(
    Guid Id,
    string Name,
    int BrandsCount = 0,
    int ModelsCount = 0,
    int VehiclesCount = 0
);
