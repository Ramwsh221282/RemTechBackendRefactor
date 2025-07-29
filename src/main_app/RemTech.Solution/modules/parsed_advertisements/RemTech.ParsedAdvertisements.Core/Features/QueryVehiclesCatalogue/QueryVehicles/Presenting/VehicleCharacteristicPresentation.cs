namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;

public sealed record VehicleCharacteristicPresentation(
    string VehicleId,
    Guid CharacteristicId,
    string Name,
    string Value,
    string Measure
);
