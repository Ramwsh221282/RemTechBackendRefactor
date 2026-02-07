using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Spares.Domain.Models;

namespace Spares.Domain.Contracts;

/// <summary>
/// Провайдер для получения идентификатора региона по адресу запчасти.
/// </summary>
public interface ISpareAddressProvider
{
	/// <summary>
	/// Ищет идентификатор региона по адресу запчасти.
	/// </summary>
	Task<Result<Guid>> SearchRegionId(string address, CancellationToken ct = default);

    /// <summary>
    /// Ищет адреса для запчастей.
    /// </summary>
    /// <returns></returns>
    Task<IReadOnlyList<Spare>> SearchAddressesForEachSpare(IEnumerable<Spare> spares, CancellationToken ct = default);
}
