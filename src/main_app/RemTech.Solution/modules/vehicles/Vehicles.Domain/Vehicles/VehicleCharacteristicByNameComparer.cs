namespace Vehicles.Domain.Vehicles;

/// <summary>
/// Сравниватель характеристик транспортных средств по имени.
/// </summary>
public sealed class VehicleCharacteristicByNameComparer : IEqualityComparer<VehicleCharacteristic>
{
	/// <summary>
	/// Сравнивает две характеристики транспортных средств по имени.
	/// </summary>
	/// <param name="x">Первая характеристика транспортного средства.</param>
	/// <param name="y">Вторая характеристика транспортного средства.</param>
	/// <returns>True, если имена характеристик совпадают; в противном случае false.</returns>
	public bool Equals(VehicleCharacteristic? x, VehicleCharacteristic? y)
	{
		return x is null || y is null ? false : x.Name == y.Name;
	}

	/// <summary>
	/// Возвращает хэш-код характеристики транспортного средства.
	/// </summary>
	/// <param name="obj">Характеристика транспортного средства.</param>
	/// <returns>Хэш-код характеристики транспортного средства.</returns>
	public int GetHashCode(VehicleCharacteristic obj) =>
		HashCode.Combine(obj.VehicleId, obj.CharacteristicId, obj.Name);
}
