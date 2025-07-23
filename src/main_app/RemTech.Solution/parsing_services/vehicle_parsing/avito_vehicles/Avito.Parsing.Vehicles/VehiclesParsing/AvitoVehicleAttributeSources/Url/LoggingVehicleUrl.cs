using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;
using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Url;

public sealed class LoggingVehicleUrl : IParsedVehicleUrlSource
{
    private readonly ICustomLogger _log;
    private readonly IParsedVehicleUrlSource _source;

    public LoggingVehicleUrl(ICustomLogger log, IParsedVehicleUrlSource source)
    {
        _log = log;
        _source = source;
    }
    
    public async Task<ParsedVehicleUrl> Read()
    {
        ParsedVehicleUrl url = await _source.Read();
        if (url)
            _log.Info("Vehicle url: {0}.", (string)url);
        else
            _log.Warn("Unable to read vehicle url.");
        return url;
    }
}