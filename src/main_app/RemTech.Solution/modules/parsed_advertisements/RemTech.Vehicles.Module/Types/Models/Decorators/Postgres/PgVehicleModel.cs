using Npgsql;
using RemTech.Core.Shared.Exceptions;

namespace RemTech.Vehicles.Module.Types.Models.Decorators.Postgres;

public sealed class PgVehicleModel(NpgsqlConnection connection, VehicleModel origin)
    : VehicleModel(origin)
{
    public async Task<VehicleModel> SaveAsync(
        PgVehicleModelFromStoreCommand command,
        CancellationToken ct = default
    )
    {
        return await command.Fetch(connection, ct);
    }

    public async Task<VehicleModel> SaveAsync(
        PgVehicleModelToStoreCommand command,
        CancellationToken ct = default
    )
    {
        int affected = await command.Execute(connection, ct);
        return affected != 1 ? throw new OperationException("Дубликат модели по названияю") : this;
    }

    public PgVehicleModelFromStoreCommand FromStoreCommand() => new(Name);

    public PgVehicleModelToStoreCommand ToStoreCommand() => new(Identity, Name);
}
