namespace Vehicles.Domain.Vehicles;

public sealed class VehicleCharacteristicByNameComparer : IEqualityComparer<VehicleCharacteristic>
{
	public bool Equals(VehicleCharacteristic? x, VehicleCharacteristic? y)
	{
		if (x is null || y is null)
			return false;
		return x.Name == y.Name;
	}

	public int GetHashCode(VehicleCharacteristic obj)
	{
		return HashCode.Combine(obj.VehicleId, obj.CharacteristicId, obj.Name);
	}
}
