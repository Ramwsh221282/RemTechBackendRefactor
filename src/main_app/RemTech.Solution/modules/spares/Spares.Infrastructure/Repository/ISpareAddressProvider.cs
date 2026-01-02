using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Infrastructure.Repository;

public interface ISpareAddressProvider
{
    Task<Result<Guid>> SearchRegionId(string address, CancellationToken ct = default);
}