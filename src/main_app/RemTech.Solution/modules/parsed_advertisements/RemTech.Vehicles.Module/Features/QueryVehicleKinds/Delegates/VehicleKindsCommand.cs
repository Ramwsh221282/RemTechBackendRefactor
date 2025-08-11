using Npgsql;
using Shared.Infrastructure.Module.Postgres.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryVehicleKinds.Delegates;

public delegate AsyncPreparedCommand VehicleKindsCommand(NpgsqlConnection connection);
