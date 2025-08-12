using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.Vehicles.Module.Types.Characteristics.Ports.Storage;

namespace RemTech.Vehicles.Module.Types.Characteristics.Adapters.Storage.Postgres;

internal sealed class PgCharacteristicsStorage(NpgsqlDataSource connectionSource)
    : IPgCharacteristicsStorage
{
    public async Task<Characteristic> Stored(Characteristic ctx, CancellationToken ct = default)
    {
        PgCharacteristicToStoreCommand command = ctx.ToStoreCommand();
        await using NpgsqlConnection connection = await connectionSource.OpenConnectionAsync(ct);
        return await command.Execute(connection, ct) != 1
            ? throw new OperationException("Duplicated characteristic name")
            : ctx;
    }
}
