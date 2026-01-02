namespace Vehicles.Domain.Vehicles.Contracts;

public interface IVehiclesListPersister
{
    Task<int> Persist(IEnumerable<VehiclePersistInfo> infos, CancellationToken ct = default);
}