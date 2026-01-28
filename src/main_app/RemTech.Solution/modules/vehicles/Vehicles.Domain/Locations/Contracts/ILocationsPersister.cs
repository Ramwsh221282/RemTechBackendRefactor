using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Locations.Contracts;

/// <summary>
/// Контракт для сохранения локаций.
/// </summary>
public interface ILocationsPersister
{
	/// <summary>
	/// Сохраняет локацию.
	/// </summary>
	/// <param name="location">Локация для сохранения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат операции сохранения локации.</returns>
	Task<Result<Location>> Save(Location location, CancellationToken ct = default);
}
