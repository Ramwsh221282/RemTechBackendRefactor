using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehicleKinds.Delegates;

public delegate Task<VehicleKindPresentationReader> VehicleKindsReader(
    AsyncPreparedCommand command,
    CancellationToken ct = default
);
