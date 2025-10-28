using ParsedAdvertisements.Domain.VehicleContext;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Storage;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Adapters.Storage.VehicleContext;

public sealed class VehiclesStorage : IVehiclesStorage
{
    public Task Save(Vehicle vehicle, ITransactionManager txn, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}