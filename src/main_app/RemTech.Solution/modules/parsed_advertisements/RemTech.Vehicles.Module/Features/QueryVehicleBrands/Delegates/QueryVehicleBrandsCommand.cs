using Npgsql;
using Shared.Infrastructure.Module.Postgres.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryVehicleBrands.Delegates;

public delegate AsyncDbReaderCommand QueryVehicleBrandsCommand(
    NpgsqlConnection connection,
    Guid id
);
