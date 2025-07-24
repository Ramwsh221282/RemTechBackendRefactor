using Parsing.Vehicles.Grpc.Recognition.Measurements;

namespace Parsing.Vehicles.Grpc.Recognition;

public sealed class Characteristic : IRecognizedCharacteristic
{
    private readonly string _name;
    private readonly string _value;
    private readonly IMeasurement _measurement;

    public Characteristic(string name, string value)
    {
        _name = name;
        _value = value;
        _measurement = new NoMeasurement();
    }

    public Characteristic(string name, string value, IMeasurement measurement)
    {
        _name = name;
        _value = value;
        _measurement = measurement;
    }

    public Characteristic()
    {
        _name = string.Empty;
        _value = string.Empty;
        _measurement = new NoMeasurement();
    }

    public Characteristic(Characteristic origin, IMeasurement measurement)
    {
        _name = origin._name;
        _value = origin._value;
        _measurement = origin._measurement;
    }

    public string ReadName() => _name;
    public string ReadValue() => _value;
    public IMeasurement Measurement() => _measurement;

    public static implicit operator bool(Characteristic characteristic)
    {
        return !string.IsNullOrWhiteSpace(characteristic._value) && !string.IsNullOrWhiteSpace(characteristic._name);
    }
}