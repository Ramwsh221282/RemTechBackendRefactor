using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.ParsedAdvertisements.Core.Types.Brands.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Types.Brands.Decorators.Postgres;

public sealed class PgVehicleBrandStoreCommand(VehicleBrandIdentity identity)
{
    private readonly string _sql = string.Intern(
        """
        INSERT INTO parsed_advertisements_module.vehicle_brands(id, text)
        VALUES(@id, @text)
        ON CONFLICT(text) DO NOTHING;
        """
    );

    public async Task<int> Execute(NpgsqlConnection connection, CancellationToken ct = default)
    {
        if (!identity.ReadText())
            throw new OperationException("Нельзя добавить бренд. Название бренда пустое.");
        if (!identity.ReadId())
            throw new OperationException("Нельзя добавить бренд. Идентификатор бренда пустой.");
        return await new AsyncExecutedCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, _sql))
                    .With("@id", (Guid)identity.ReadId())
                    .With("@text", (string)identity.ReadText())
            )
        ).AsyncExecuted(ct);
    }
}
