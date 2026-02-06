using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Infrastructure.Repository;

/// <summary>
/// Провайдер для получения идентификатора региона по адресу запчасти.
/// </summary>
public interface ISpareAddressProvider
{
	/// <summary>
	/// Ищет идентификатор региона по адресу запчасти.
	/// </summary>
	/// <param name="address">Адрес запчасти для поиска региона.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат с идентификатором региона.</returns>
	Task<Result<Guid>> SearchRegionId(string address, CancellationToken ct = default);
}
