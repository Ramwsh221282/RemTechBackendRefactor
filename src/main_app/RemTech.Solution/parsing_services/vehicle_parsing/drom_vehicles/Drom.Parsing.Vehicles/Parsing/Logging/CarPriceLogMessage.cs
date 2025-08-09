namespace Drom.Parsing.Vehicles.Parsing.Logging;

public sealed class CarPriceLogMessage
{
    private readonly long _value;
    private readonly bool _isNds;

    public CarPriceLogMessage(long value, bool isNds)
    {
        _value = value;
        _isNds = isNds;
    }

    public void Log(Serilog.ILogger logger)
    {
        logger.Information("Car price {value} {isNds}.", _value, _isNds);
    }
}
