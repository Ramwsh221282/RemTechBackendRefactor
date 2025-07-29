using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Types;
using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;
using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData.Types;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.Types;

public sealed record VehiclesCataloguePresentation(
    IEnumerable<VehiclePresentation> Vehicles,
    VehicleCharacteristicsDictionary Characteristics,
    VehiclesAggregatedDataPresentation AggregatedData
);
