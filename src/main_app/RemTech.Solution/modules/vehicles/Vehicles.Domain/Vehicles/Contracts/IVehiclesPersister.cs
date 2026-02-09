using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles.Contracts;

/// <summary>
/// Контракт для сохранения информации о транспортных средствах.
/// </summary>
public interface IVehiclesPersister
{
	/// <summary>
	/// Сохраняет информацию о транспортном средстве.
	/// </summary>
	/// <param name="info">Информация о транспортном средстве для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения информации о транспортном средстве.</returns>
	Task<Result<Unit>> Persist(VehiclePersistInfo info, CancellationToken ct = default);
}
