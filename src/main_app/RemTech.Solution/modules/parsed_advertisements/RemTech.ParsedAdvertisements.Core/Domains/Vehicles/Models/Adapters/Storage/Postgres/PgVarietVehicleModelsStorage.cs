using System.Diagnostics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Ports.Storage.Postgres;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Adapters.Storage.Postgres;

public sealed class PgVarietVehicleModelsStorage : IPgVehicleModelsStorage
{
    private readonly Queue<IPgVehicleModelsStorage> _storages = [];

    public PgVarietVehicleModelsStorage With(IPgVehicleModelsStorage storage)
    {
        _storages.Enqueue(storage);
        return this;
    }

    public async Task<VehicleModel> Get(VehicleModel model, CancellationToken ct = default)
    {
        while (_storages.Count > 0)
        {
            IPgVehicleModelsStorage storage = _storages.Dequeue();
            try
            {
                return await storage.Get(model);
            }
            catch
            {
                // ignored
            }
        }

        throw new UnreachableException("Unable to store vehicle model.");
    }
}