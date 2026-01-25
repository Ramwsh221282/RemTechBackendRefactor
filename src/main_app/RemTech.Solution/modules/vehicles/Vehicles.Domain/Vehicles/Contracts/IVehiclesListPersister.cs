namespace Vehicles.Domain.Vehicles.Contracts;

public interface IVehiclesListPersister
{
    public Task<int> Persist(IEnumerable<VehiclePersistInfo> infos, CancellationToken ct = default);
}
