namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;

public sealed class KeyValueVehicleCharacteristics
{
    private readonly Dictionary<string, string> _characteristics;

    public KeyValueVehicleCharacteristics()
    {
        _characteristics = [];
    }

    public KeyValueVehicleCharacteristics With((string key, string value) pair)
    {
        if (_characteristics.ContainsKey(pair.key))
            return this;
        _characteristics.Add(pair.key, pair.value);
        return this;
    }

    public int Amount() => _characteristics.Count;

    public (string key, string value)[] Read()
    {
        (string key, string value)[] entries = new (string key, string value)[_characteristics.Count];
        int index = 0;
        foreach (KeyValuePair<string, string> item in _characteristics)
        {
            entries[index] = (item.Key, item.Value);
            index++;
        }
        
        return entries;
    }

    public static implicit operator bool(KeyValueVehicleCharacteristics characteristics)
    {
        return characteristics.Amount() > 0;
    }
}