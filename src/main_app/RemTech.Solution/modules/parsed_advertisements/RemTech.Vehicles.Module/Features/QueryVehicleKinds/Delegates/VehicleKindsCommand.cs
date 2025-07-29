using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryVehicleKinds.Delegates;

public delegate AsyncPreparedCommand VehicleKindsCommand(NpgsqlConnection connection);
