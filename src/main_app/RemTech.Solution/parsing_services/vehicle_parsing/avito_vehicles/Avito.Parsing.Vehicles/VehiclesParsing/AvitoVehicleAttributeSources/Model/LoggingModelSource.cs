using Parsing.SDK.Logging;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Model;

public sealed class LoggingModelSource : IParsedVehicleModelSource
{
    private readonly IParsingLog _log;
    private readonly IParsedVehicleModelSource _origin;

    public LoggingModelSource(IParsingLog log, IParsedVehicleModelSource origin)
    {
        _log = log;
        _origin = origin;
    }
    
    public async Task<ParsedVehicleModel> Read()
    {
        ParsedVehicleModel model = await _origin.Read();
        if (model)
            _log.Info("Vehicle model: {0}.", (string)model);
        else
            _log.Info("Unable to get vehicle model.");
        return model;
    }
}