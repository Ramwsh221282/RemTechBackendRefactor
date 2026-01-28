using Vehicles.Domain.Characteristics;

namespace Vehicles.Domain.Vehicles;

/// <summary>
/// Характеристика транспортного средства.
/// </summary>
/// <param name="vehicle">Транспортное средство.</param>
/// <param name="characteristic">Характеристика транспортного средства.</param>
/// <param name="value">Значение характеристики транспортного средства.</param>
public sealed class VehicleCharacteristic(
	Vehicle vehicle,
	Characteristic characteristic,
	VehicleCharacteristicValue value
)
{
	/// <summary>
	/// Идентификатор транспортного средства.
	/// </summary>
	public VehicleId VehicleId { get; } = vehicle.Id;

	/// <summary>
	/// Идентификатор характеристики транспортного средства.
	/// </summary>
	public CharacteristicId CharacteristicId { get; } = characteristic.Id;

	/// <summary>
	/// Значение характеристики транспортного средства.
	/// </summary>
	public VehicleCharacteristicValue Value { get; } = value;

	/// <summary>
	/// Название характеристики транспортного средства.
	/// </summary>
	public CharacteristicName Name { get; } = characteristic.Name;
}
