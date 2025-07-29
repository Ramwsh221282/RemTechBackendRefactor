namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;

public sealed record VehicleCharacteristicsPresentation(
    IEnumerable<VehicleCharacteristicPresentation> Characteristics
);
