namespace RemTech.Vehicles.Module.Features.QueryVehicleBrands.Types;

public record VehicleBrandPresentation(
    Guid Id,
    string Name,
    int ModelsCount = 0,
    int VehiclesCount = 0
);
