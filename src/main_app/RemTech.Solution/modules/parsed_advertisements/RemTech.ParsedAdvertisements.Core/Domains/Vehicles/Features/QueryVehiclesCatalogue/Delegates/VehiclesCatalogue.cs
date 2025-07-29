using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.Types;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.Delegates;

public delegate Task<VehiclesCataloguePresentation> VehiclesCatalogue(
    VehiclesOfCatalogue vehicles,
    CharacteristicsOfCatalogue characteristics,
    AggregatedDataOfCatalogue aggregatedData
);
