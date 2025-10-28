using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.VehicleContext.Ports.Storage;

public interface IVehiclesStorage
{
    Task Save(Vehicle vehicle, ITransactionManager txn, CancellationToken ct = default);
}