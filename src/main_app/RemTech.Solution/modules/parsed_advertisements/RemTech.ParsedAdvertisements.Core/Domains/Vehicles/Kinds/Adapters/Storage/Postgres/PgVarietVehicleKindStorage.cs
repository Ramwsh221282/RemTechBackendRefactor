using System.Diagnostics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports.Storage.Postgres;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Adapters.Storage.Postgres;

public sealed class PgVarietVehicleKindStorage : IPgVehicleKindsStorage
{
    private readonly Queue<IPgVehicleKindsStorage> _storages = [];

    public PgVarietVehicleKindStorage With(IPgVehicleKindsStorage storage)
    {
        _storages.Enqueue(storage);
        return this;
    }

    public async Task<VehicleKind> Read(VehicleKind kind, CancellationToken ct = default)
    {
        while (_storages.Count > 0)
        {
            try
            {
                IPgVehicleKindsStorage storage = _storages.Dequeue();
                return await storage.Read(kind, ct);
            }
            catch
            {
                // ignored
            }
        }

        throw new UnreachableException("Unable to read vehicle kind from variet storages.");
    }
}