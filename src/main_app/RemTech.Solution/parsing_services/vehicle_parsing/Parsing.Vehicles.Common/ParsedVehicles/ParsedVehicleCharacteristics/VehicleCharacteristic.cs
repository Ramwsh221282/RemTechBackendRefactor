namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;

public interface IVehicleCharacteristic
{
    string Name();
    string Value();
    string Measurement();
}




public sealed class VehicleCharacteristic
{
    private readonly string _name;
    private readonly string _value;

    public VehicleCharacteristic(string name, string value)
    {
        _name = name;
        _value = value;
    }

    public VehicleCharacteristic()
    {
        _name = string.Empty;
        _value = string.Empty;
    }

    public string Name() => _name;
    public string Value() => _value;
}

