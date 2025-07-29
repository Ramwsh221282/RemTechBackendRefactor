using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.Delegates;

public delegate Task<IEnumerable<VehiclePresentation>> VehiclesOfCatalogue();
