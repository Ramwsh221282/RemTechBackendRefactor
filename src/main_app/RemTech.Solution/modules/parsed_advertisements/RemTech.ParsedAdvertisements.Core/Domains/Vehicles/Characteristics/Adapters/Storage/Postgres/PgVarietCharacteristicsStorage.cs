using System.Diagnostics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports.Storage;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Adapters.Storage.Postgres;

public sealed class PgVarietCharacteristicsStorage : IPgCharacteristicsStorage
{
    private readonly Queue<IPgCharacteristicsStorage> _storages = [];

    public PgVarietCharacteristicsStorage With(IPgCharacteristicsStorage storage)
    {
        _storages.Enqueue(storage);
        return this;
    }
    
    public async Task<ICharacteristic> Stored(UnstructuredCharacteristic unstructured, CancellationToken ct = default)
    {
        while (_storages.Count > 0)
        {
            IPgCharacteristicsStorage storage = _storages.Dequeue();
            try
            {
                return await storage.Stored(unstructured, ct);
            }
            catch
            {
                // ignored
            }
        }

        throw new UnreachableException("Unable to store characteristic.");
    }
}