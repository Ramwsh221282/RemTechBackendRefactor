using Npgsql;
using RemTech.Core.Shared.Exceptions;

namespace RemTech.ParsedAdvertisements.Core.Types.Kinds.Decorators.Postgres;

public sealed class PgVehicleKind(NpgsqlConnection connection, VehicleKind kind) : VehicleKind(kind)
{
    public async Task<VehicleKind> SaveAsync(
        PgVehicleKindToStoreCommand command,
        CancellationToken ct = default
    )
    {
        int affected = await command.Execute(connection, ct);
        return affected != 1
            ? throw new OperationException("Дубликат типа техники по названию")
            : this;
    }

    public async Task<VehicleKind> SaveAsync(
        PgVehicleKindFromStoreCommand command,
        CancellationToken ct = default
    )
    {
        return await command.Fetch(connection, ct);
    }

    public PgVehicleKindToStoreCommand ToStoreCommand() =>
        new(Identity.ReadText(), Identity.ReadId());

    public PgVehicleKindFromStoreCommand FromStoreCommand() => new(Identity.ReadText());
}
