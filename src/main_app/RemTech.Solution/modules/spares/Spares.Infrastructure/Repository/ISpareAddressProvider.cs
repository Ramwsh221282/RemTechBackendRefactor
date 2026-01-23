using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Infrastructure.Repository;

public interface ISpareAddressProvider
{
	public Task<Result<Guid>> SearchRegionId(string address, CancellationToken ct = default);
}
