using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports.Storage;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Adapters.Storage.Postgres;

public sealed class PgCharacteristicsStorage(PgConnectionSource connectionSource) : IPgCharacteristicsStorage
{
    public async Task<ICharacteristic> Stored(UnstructuredCharacteristic unstructured, CancellationToken ct = default)
    {
        if (!unstructured.TryStructure(out ValuedCharacteristic valued))
            throw new ApplicationException("Characteristic is unstructured.");
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        return await valued.StoreCommand().Execute(connection, ct) != 1
            ? throw new ApplicationException("Duplicated characteristic name")
            : valued;
    }
}