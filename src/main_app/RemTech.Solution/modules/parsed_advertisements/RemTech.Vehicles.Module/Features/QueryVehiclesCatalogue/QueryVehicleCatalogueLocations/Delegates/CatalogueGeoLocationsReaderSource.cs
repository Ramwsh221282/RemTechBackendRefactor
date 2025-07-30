using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCatalogueLocations.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCatalogueLocations.Delegates;

public delegate Task<CatalogueGeoLocationsReader> CatalogueGeoLocationsReaderSource(
    AsyncDbReaderCommand command,
    CancellationToken ct = default
);
