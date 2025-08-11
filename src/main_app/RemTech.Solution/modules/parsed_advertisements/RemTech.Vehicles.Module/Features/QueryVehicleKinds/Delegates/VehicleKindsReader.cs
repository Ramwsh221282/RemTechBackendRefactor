using RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types;
using Shared.Infrastructure.Module.Postgres.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryVehicleKinds.Delegates;

public delegate Task<VehicleKindPresentationReader> VehicleKindsReader(
    AsyncPreparedCommand command,
    CancellationToken ct = default
);
