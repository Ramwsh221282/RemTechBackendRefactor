using System.Diagnostics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports.Storage.Postgres;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Adapters.Storage.Postgres;

public sealed class PgVarietVehicleGeosStorage : IPgVehicleGeosStorage
{
    private readonly Queue<IPgVehicleGeosStorage> _storages = [];

    public PgVarietVehicleGeosStorage With(IPgVehicleGeosStorage storage)
    {
        _storages.Enqueue(storage);
        return this;
    }

    public async Task<GeoLocation> Get(GeoLocation location, CancellationToken ct = default)
    {
        while (_storages.Count > 0)
        {
            IPgVehicleGeosStorage storage = _storages.Dequeue();
            try
            {
                return await storage.Get(location, ct);
            }
            catch
            {
                // ignored
            }
        }

        throw new UnreachableException("Unable to store location.");
    }
}