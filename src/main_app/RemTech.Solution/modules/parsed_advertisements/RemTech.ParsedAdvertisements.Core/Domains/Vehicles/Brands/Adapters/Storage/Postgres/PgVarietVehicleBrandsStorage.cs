using System.Diagnostics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports.Storage;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Adapters.Storage.Postgres;

public sealed class PgVarietVehicleBrandsStorage : IPgVehicleBrandsStorage
{
    private readonly Queue<IPgVehicleBrandsStorage> _storages = [];

    public PgVarietVehicleBrandsStorage With(IPgVehicleBrandsStorage storage)
    {
        _storages.Enqueue(storage);
        return this;
    }

    public async Task<VehicleBrand> Get(VehicleBrand brand, CancellationToken ct = default)
    {
        while (_storages.Count > 0)
        {
            IPgVehicleBrandsStorage storage = _storages.Dequeue();
            try
            {
                return await storage.Get(brand, ct);
            }
            catch
            {
                // ignored
            }
        }

        throw new UnreachableException("Unable to process vehicle brand into storage.");
    }
}