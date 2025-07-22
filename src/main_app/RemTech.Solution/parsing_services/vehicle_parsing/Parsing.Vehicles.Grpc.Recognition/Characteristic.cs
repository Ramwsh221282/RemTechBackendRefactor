namespace Parsing.Vehicles.Grpc.Recognition;

public sealed class Characteristic
{
    private readonly string _name;
    private readonly string _value;

    public Characteristic(string name, string value)
    {
        _name = name;
        _value = value;
    }

    public Characteristic()
    {
        _name = string.Empty;
        _value = string.Empty;
    }

    public string ReadName() => _name;
    public string ReadValue() => _value;

    public static implicit operator bool(Characteristic characteristic)
    {
        return !string.IsNullOrWhiteSpace(characteristic._value) && !string.IsNullOrWhiteSpace(characteristic._name);
    }
}