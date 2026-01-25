namespace Vehicles.Domain.Vehicles;

public sealed class VehicleCharacteristicByNameComparer : IEqualityComparer<VehicleCharacteristic>
{
    public bool Equals(VehicleCharacteristic? x, VehicleCharacteristic? y)
    {
        return x is null || y is null ? false : x.Name == y.Name;
    }

    public int GetHashCode(VehicleCharacteristic obj) =>
        HashCode.Combine(obj.VehicleId, obj.CharacteristicId, obj.Name);
}
