using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.Delegates;

public delegate Task<VehiclesCataloguePresentation> VehiclesCatalogue(
    VehiclesOfCatalogue vehicles,
    CharacteristicsOfCatalogue characteristics,
    AggregatedDataOfCatalogue aggregatedData
);
