using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Locations.Contracts;

public interface ILocationsPersister
{
	Task<Result<Location>> Save(Location location, CancellationToken ct = default);
}
