namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;

public sealed record VehiclePresentation(
    VehicleIdentityPresentation Identity,
    VehicleKindPresentation Kind,
    VehicleBrandPresentation Brand,
    VehicleModelPresentation Model,
    VehicleRegionPresentation Region,
    VehiclePricePresentation Price,
    VehiclePhotosPresentation Photos,
    VehicleCharacteristicsPresentation Characteristics
);
