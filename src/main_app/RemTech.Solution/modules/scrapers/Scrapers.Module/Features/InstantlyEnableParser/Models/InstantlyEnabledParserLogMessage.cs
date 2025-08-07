namespace Scrapers.Module.Features.InstantlyEnableParser.Models;

internal sealed class InstantlyEnabledParserLogMessage
{
    private readonly string _name;
    private readonly string _type;

    public InstantlyEnabledParserLogMessage(string name, string type)
    {
        _name = name;
        _type = type;
    }

    public void Log(Serilog.ILogger logger)
    {
        logger.Information("Парсер {Name} {Type} был немедленно включен.", _name, _type);
    }
}
