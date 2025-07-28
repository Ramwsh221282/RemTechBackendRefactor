using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using Serilog;

namespace Parsing.Vehicles.DbSearch.VehicleModels;

public sealed class LoggingVehicleModelDbSearch : IVehicleModelDbSearch
{
    private readonly ILogger _logger;
    private readonly IVehicleModelDbSearch _origin;

    public LoggingVehicleModelDbSearch(ILogger logger, IVehicleModelDbSearch origin)
    {
        _logger = logger;
        _origin = origin;
    }

    public async Task<ParsedVehicleModel> Search(string text)
    {
        ParsedVehicleModel model = await _origin.Search(text);
        if (model)
            _logger.Information("Search vehicle model: {0}. Parameter: {1}.", (string)model, text);
        else
            _logger.Warning("Unable to search vehicle model. Parameter: {0}.", text);
        return model;
    }
}