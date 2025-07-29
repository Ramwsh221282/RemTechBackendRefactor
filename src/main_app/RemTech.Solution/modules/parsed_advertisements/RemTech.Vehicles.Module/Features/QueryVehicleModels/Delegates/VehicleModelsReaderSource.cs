using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Features.QueryVehicleModels.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehicleModels.Delegates;

public delegate Task<VehicleModelPresentationReader> VehicleModelsReaderSource(
    AsyncDbReaderCommand command,
    CancellationToken ct = default
);
