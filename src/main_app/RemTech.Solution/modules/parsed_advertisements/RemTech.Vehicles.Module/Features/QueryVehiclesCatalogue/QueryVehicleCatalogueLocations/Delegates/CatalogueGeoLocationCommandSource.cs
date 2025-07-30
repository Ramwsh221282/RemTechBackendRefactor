using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCatalogueLocations.Delegates;

public delegate AsyncDbReaderCommand CatalogueGeoLocationCommandSource(
    NpgsqlConnection connection,
    VehiclesQueryRequest request
);
