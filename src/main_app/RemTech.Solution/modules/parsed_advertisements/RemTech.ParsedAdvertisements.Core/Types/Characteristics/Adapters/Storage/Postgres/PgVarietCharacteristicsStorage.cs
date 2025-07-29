using RemTech.Core.Shared.Exceptions;
using RemTech.ParsedAdvertisements.Core.Types.Characteristics.Ports.Storage;

namespace RemTech.ParsedAdvertisements.Core.Types.Characteristics.Adapters.Storage.Postgres;

public sealed class PgVarietCharacteristicsStorage : IPgCharacteristicsStorage
{
    private readonly Queue<IPgCharacteristicsStorage> _storages = [];

    public PgVarietCharacteristicsStorage With(IPgCharacteristicsStorage storage)
    {
        _storages.Enqueue(storage);
        return this;
    }

    public async Task<Characteristic> Stored(Characteristic ctx, CancellationToken ct = default)
    {
        while (_storages.Count > 0)
        {
            IPgCharacteristicsStorage storage = _storages.Dequeue();
            try
            {
                return await storage.Stored(ctx, ct);
            }
            catch
            {
                // ignored
            }
        }

        throw new OperationException("Невозможно добавить характеристику.");
    }
}
