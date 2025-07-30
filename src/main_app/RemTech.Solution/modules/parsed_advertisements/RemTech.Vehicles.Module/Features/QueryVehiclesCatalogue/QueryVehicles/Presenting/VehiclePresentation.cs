using RemTech.Vehicles.Module.Features.QueryVehicleBrands.Types;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types;
using RemTech.Vehicles.Module.Features.QueryVehicleModels.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;

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
