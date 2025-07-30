using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCatalogueLocations.Types;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.Delegates;

public delegate Task<VehiclesCataloguePresentation> VehiclesCatalogue(
    VehiclesOfCatalogue vehicles,
    CharacteristicsOfCatalogue characteristics,
    AggregatedDataOfCatalogue aggregatedData,
    GeoLocationsOfCatalogue geoLocations
);

public delegate Task<IEnumerable<CatalogueGeoLocationPresentation>> GeoLocationsOfCatalogue();
