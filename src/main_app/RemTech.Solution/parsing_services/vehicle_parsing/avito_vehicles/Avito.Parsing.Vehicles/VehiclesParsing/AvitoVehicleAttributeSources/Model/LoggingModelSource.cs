using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Model;

public sealed class LoggingModelSource : IParsedVehicleModelSource
{
    private readonly ICustomLogger _log;
    private readonly IParsedVehicleModelSource _origin;

    public LoggingModelSource(ICustomLogger log, IParsedVehicleModelSource origin)
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