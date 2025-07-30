using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCatalogueLocations.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCatalogueLocations.Delegates;

public delegate Task<
    IEnumerable<CatalogueGeoLocationPresentation>
> CatalogueGeoLocationsReadingSource(
    CatalogueGeoLocationsReader reader,
    CancellationToken ct = default
);
