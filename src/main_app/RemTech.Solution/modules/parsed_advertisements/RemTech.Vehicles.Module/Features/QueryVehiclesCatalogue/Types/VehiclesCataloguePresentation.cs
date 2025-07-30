using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCatalogueLocations.Types;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Types;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.Types;

public sealed record VehiclesCataloguePresentation(
    IEnumerable<VehiclePresentation> Vehicles,
    VehicleCharacteristicsDictionary Characteristics,
    VehiclesAggregatedDataPresentation AggregatedData,
    IEnumerable<CatalogueGeoLocationPresentation> GeoLocations
);
