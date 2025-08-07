using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Model;

public sealed class LoggingModelSource : IParsedVehicleModelSource
{
    private readonly Serilog.ILogger _log;
    private readonly IParsedVehicleModelSource _origin;

    public LoggingModelSource(Serilog.ILogger log, IParsedVehicleModelSource origin)
    {
        _log = log;
        _origin = origin;
    }

    public async Task<ParsedVehicleModel> Read()
    {
        ParsedVehicleModel model = await _origin.Read();
        if (model)
            _log.Information("Vehicle model: {0}.", (string)model);
        else
            _log.Warning("Unable to get vehicle model.");
        return model;
    }
}
