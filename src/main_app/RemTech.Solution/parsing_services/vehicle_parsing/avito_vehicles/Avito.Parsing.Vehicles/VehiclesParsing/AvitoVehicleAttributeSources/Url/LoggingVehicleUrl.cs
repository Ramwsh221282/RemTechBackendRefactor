using Parsing.SDK.Logging;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Url;

public sealed class LoggingVehicleUrl : IParsedVehicleUrlSource
{
    private readonly IParsingLog _log;
    private readonly IParsedVehicleUrlSource _source;

    public LoggingVehicleUrl(IParsingLog log, IParsedVehicleUrlSource source)
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
            _log.Warning("Unable to read vehicle url.");
        return url;
    }
}