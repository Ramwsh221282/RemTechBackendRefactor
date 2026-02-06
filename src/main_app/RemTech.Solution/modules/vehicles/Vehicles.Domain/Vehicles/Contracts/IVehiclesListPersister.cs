namespace Vehicles.Domain.Vehicles.Contracts;

/// <summary>
/// Контракт для сохранения списка информации о транспортных средствах.
/// </summary>
public interface IVehiclesListPersister
{
	/// <summary>
	/// Сохраняет список информации о транспортных средствах.
	/// </summary>
	/// <param name="infos">Список информации о транспортных средствах для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Количество успешно сохранённых записей.</returns>
	Task<int> Persist(IEnumerable<VehiclePersistInfo> infos, CancellationToken ct = default);
}
