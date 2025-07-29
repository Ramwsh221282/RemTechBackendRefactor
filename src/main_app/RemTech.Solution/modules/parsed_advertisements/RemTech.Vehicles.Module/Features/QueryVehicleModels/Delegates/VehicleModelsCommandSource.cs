using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryVehicleModels.Delegates;

public delegate AsyncDbReaderCommand VehicleModelsCommandSource(
    NpgsqlConnection connection,
    Guid kindId,
    Guid brandId
);
