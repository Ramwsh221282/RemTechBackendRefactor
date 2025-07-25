using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports.Storage;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Adapters.Storage.Postgres;

public sealed class PgCharacteristicsStorage(PgConnectionSource connectionSource) : IPgCharacteristicsStorage
{
    public async Task<Characteristic> Stored(Characteristic ctx, CancellationToken ct = default)
    {
        PgCharacteristicToStoreCommand command = ctx.ToStoreCommand();
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        return await command.Execute(connection, ct) != 1
            ? throw new OperationException("Duplicated characteristic name")
            : ctx;
    }
}