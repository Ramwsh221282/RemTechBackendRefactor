using Npgsql;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports.Storage;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Adapters.Storage.Postgres;

public sealed class PgDuplicateResolvingCharacteristicsStorage(PgConnectionSource connectionSource)
    : IPgCharacteristicsStorage
{
    public async Task<Characteristic> Stored(Characteristic ctx, CancellationToken ct = default)
    {
        PgCharacteristicFromStoreCommand command = ctx.FromStoreCommand();
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        return await command.Fetch(connection, ct);
    }
}