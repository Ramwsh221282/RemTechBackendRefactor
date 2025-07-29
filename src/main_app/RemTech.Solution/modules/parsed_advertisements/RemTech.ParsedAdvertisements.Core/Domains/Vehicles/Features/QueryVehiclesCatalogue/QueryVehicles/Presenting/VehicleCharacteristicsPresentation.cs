namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;

public sealed record VehicleCharacteristicsPresentation(
    IEnumerable<VehicleCharacteristicPresentation> Characteristics
);
