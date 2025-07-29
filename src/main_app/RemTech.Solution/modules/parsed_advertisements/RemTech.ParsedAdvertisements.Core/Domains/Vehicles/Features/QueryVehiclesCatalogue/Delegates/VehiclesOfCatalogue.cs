using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.Delegates;

public delegate Task<IEnumerable<VehiclePresentation>> VehiclesOfCatalogue();
