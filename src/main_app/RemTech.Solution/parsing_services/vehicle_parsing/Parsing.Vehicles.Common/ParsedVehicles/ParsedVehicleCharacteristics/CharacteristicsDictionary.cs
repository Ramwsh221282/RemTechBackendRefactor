namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;

public sealed class CharacteristicsDictionary : IVehicleCharacteristics
{
    private readonly Dictionary<string, VehicleCharacteristic> _characteristics;

    public CharacteristicsDictionary()
    {
        _characteristics = [];
    }

    public CharacteristicsDictionary FromOther(CharacteristicsDictionary other)
    {
        foreach (VehicleCharacteristic characteristic in other._characteristics.Values)
            AddCharacteristic(characteristic);
        return this;
    }
    
    public CharacteristicsDictionary FromEnumerable(IEnumerable<VehicleCharacteristic> characteristics)
    {
        foreach (var characteristic in characteristics)
            AddCharacteristic(characteristic);
        return this;
    }
    
    public CharacteristicsDictionary With(VehicleCharacteristic characteristic)
    {
        AddCharacteristic(characteristic);
        return this;
    }

    public int Amount() => _characteristics.Count;

    public static implicit operator bool(CharacteristicsDictionary characteristicsDictionary)
    {
        return characteristicsDictionary.Amount() > 0;
    }

    public IEnumerable<VehicleCharacteristic> Read()
    {
        return _characteristics.Values;
    }

    private void AddCharacteristic(VehicleCharacteristic characteristic)
    {
        if (string.IsNullOrWhiteSpace(characteristic.Name()) || string.IsNullOrWhiteSpace(characteristic.Value()))
            return;
        if (_characteristics.ContainsKey(characteristic.Name()))
            return;
        _characteristics.Add(characteristic.Name(), characteristic);
    }
}