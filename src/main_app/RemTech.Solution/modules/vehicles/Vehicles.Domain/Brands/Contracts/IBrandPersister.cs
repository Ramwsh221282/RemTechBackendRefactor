using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Brands.Contracts;

/// <summary>
/// Персистер брендов.
/// </summary>
public interface IBrandPersister
{
	/// <summary>
	/// Сохраняет бренд.
	/// </summary>
	/// <param name="brand">Бренд для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения бренда.</returns>
	Task<Result<Brand>> Save(Brand brand, CancellationToken ct = default);
}
