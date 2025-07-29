using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Types;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData.Types;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.Types;

public sealed record VehiclesCataloguePresentation(
    IEnumerable<VehiclePresentation> Vehicles,
    VehicleCharacteristicsDictionary Characteristics,
    VehiclesAggregatedDataPresentation AggregatedData
);
