namespace RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;

public sealed record VehicleCharacteristicsPresentation(
    IEnumerable<VehicleCharacteristicPresentation> Characteristics
);
