namespace Drom.Parsing.Vehicles.Parsing.Logging;

public sealed class CarCharacteristicLogMessage
{
    private readonly string _name;
    private readonly string _value;

    public CarCharacteristicLogMessage(string name, string value)
    {
        _name = name;
        _value = value;
    }

    public void Log(Serilog.ILogger logger)
    {
        logger.Information("Characteristic {Name} {Value}.", _name, _value);
    }
}
