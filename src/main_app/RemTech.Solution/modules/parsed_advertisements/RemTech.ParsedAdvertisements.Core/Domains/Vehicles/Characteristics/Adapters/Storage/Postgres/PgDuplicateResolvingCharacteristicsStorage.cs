using Npgsql;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports.Storage;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Adapters.Storage.Postgres;

public sealed class PgDuplicateResolvingCharacteristicsStorage(PgConnectionSource connectionSource)
    : IPgCharacteristicsStorage
{
    public async Task<ICharacteristic> Stored(UnstructuredCharacteristic unstructured, CancellationToken ct = default)
    {
        NotEmptyString name = new(unstructured.Name());
        PgCharacteristicFromStoreCommand pgCharacteristicFrom = new(name);
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        return await pgCharacteristicFrom.Fetch(connection, ct);
    }
}