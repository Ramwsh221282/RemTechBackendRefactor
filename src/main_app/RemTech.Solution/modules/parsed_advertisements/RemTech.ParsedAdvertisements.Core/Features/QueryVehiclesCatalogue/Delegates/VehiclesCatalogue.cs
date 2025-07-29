using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.Types;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.Delegates;

public delegate Task<VehiclesCataloguePresentation> VehiclesCatalogue(
    VehiclesOfCatalogue vehicles,
    CharacteristicsOfCatalogue characteristics,
    AggregatedDataOfCatalogue aggregatedData
);
