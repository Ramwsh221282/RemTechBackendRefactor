using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Types.Brands.Decorators.Postgres;

public sealed class PgVehicleBrand(PgConnectionSource connectionSource, VehicleBrand origin)
    : VehicleBrand(origin)
{
    public async Task<VehicleBrand> SaveAsync(
        PgVehicleBrandFromStoreCommand command,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        return await command.Fetch(connection, ct);
    }

    public async Task<VehicleBrand> SaveAsync(
        PgVehicleBrandStoreCommand command,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        int affected = await command.Execute(connection, ct);
        return affected != 1 ? throw new OperationException("Дубликат названия бренда.") : this;
    }

    public PgVehicleBrandFromStoreCommand FromStoreCommand() => new(Identity);

    public PgVehicleBrandStoreCommand ToStoreCommand() => new(Identity);
}
