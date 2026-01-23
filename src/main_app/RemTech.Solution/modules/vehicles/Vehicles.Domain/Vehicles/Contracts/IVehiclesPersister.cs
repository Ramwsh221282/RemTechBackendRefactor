using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles.Contracts;

public interface IVehiclesPersister
{
	public Task<Result<Unit>> Persist(VehiclePersistInfo info, CancellationToken ct = default);
}
