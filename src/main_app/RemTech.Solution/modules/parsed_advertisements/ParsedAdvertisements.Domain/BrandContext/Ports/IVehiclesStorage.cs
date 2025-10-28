using ParsedAdvertisements.Domain.VehicleContext;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.BrandContext.Ports;

public interface IVehiclesStorage
{
    Task<Status> Save(Vehicle vehicle, ITransaction txn, CancellationToken ct = default);
}
