using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Contracts;
using Vehicles.Domain.Locations;

namespace Vehicles.Domain.Vehicles.Contracts;

public sealed record VehiclePersistInfo(Vehicle Vehicle, Location Location) : IPersistable<VehiclePersistInfo>
{
	public Task<Result<VehiclePersistInfo>> SaveBy(IPersister persister, CancellationToken ct = default)
	{
		return persister.Save(this, ct);
	}
}
