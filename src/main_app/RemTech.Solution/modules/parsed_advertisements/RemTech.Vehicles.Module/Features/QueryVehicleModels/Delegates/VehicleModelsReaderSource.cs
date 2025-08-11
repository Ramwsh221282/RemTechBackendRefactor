using RemTech.Vehicles.Module.Features.QueryVehicleModels.Types;
using Shared.Infrastructure.Module.Postgres.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryVehicleModels.Delegates;

public delegate Task<VehicleModelPresentationReader> VehicleModelsReaderSource(
    AsyncDbReaderCommand command,
    CancellationToken ct = default
);
