using Npgsql;
using RemTech.Vehicles.Module.Types.Characteristics.Ports.Storage;

namespace RemTech.Vehicles.Module.Types.Characteristics.Adapters.Storage.Postgres;

internal sealed class PgDuplicateResolvingCharacteristicsStorage(NpgsqlDataSource connectionSource)
    : IPgCharacteristicsStorage
{
    public async Task<Characteristic> Stored(Characteristic ctx, CancellationToken ct = default)
    {
        PgCharacteristicFromStoreCommand command = ctx.FromStoreCommand();
        await using NpgsqlConnection connection = await connectionSource.OpenConnectionAsync(ct);
        return await command.Fetch(connection, ct);
    }
}
