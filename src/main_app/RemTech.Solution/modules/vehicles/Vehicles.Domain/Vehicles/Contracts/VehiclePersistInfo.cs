using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Contracts;
using Vehicles.Domain.Locations;

namespace Vehicles.Domain.Vehicles.Contracts;

/// <summary>
/// Информация для персистенции транспортного средства вместе с его локацией.
/// </summary>
/// <param name="Vehicle">Транспортное средство.</param>
/// <param name="Location">Локация транспортного средства.</param>
public sealed record VehiclePersistInfo(Vehicle Vehicle, Location Location) : IPersistable<VehiclePersistInfo>
{
	/// <summary>
	/// Сохраняет информацию о транспортном средстве с помощью указанного персистера.
	/// </summary>
	/// <param name="persister">Персистер для сохранения данных.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения информации о транспортном средстве.</returns>
	public Task<Result<VehiclePersistInfo>> SaveBy(IPersister persister, CancellationToken ct = default) =>
		persister.Save(this, ct);
}
