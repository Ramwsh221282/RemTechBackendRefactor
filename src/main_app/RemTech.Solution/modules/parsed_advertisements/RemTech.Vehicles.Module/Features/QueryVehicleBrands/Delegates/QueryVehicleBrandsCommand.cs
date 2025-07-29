using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryVehicleBrands.Delegates;

public delegate AsyncDbReaderCommand QueryVehicleBrandsCommand(
    NpgsqlConnection connection,
    Guid id
);
