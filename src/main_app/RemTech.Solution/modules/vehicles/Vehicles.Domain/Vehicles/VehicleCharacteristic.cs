using Vehicles.Domain.Characteristics;

namespace Vehicles.Domain.Vehicles;

public sealed class VehicleCharacteristic(
	Vehicle vehicle,
	Characteristic characteristic,
	VehicleCharacteristicValue value
)
{
	public VehicleId VehicleId { get; } = vehicle.Id;
	public CharacteristicId CharacteristicId { get; } = characteristic.Id;
	public VehicleCharacteristicValue Value { get; } = value;
	public CharacteristicName Name { get; } = characteristic.Name;
}
