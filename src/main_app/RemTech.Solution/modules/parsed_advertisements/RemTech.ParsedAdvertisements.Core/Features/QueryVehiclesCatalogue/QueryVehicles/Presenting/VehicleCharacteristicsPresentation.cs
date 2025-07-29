namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;

public sealed record VehicleCharacteristicsPresentation(
    IEnumerable<VehicleCharacteristicPresentation> Characteristics
);
