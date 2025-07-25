using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using RemTech.Logging.Library;

namespace Parsing.Vehicles.DbSearch.VehicleKinds;

public sealed class LoggingVehicleKindSearch : IVehicleKindDbSearch
{
    private readonly ICustomLogger _logger;
    private readonly IVehicleKindDbSearch _origin;

    public LoggingVehicleKindSearch(ICustomLogger logger, IVehicleKindDbSearch origin)
    {
        _logger = logger;
        _origin = origin;
    }
    
    public async Task<ParsedVehicleKind> Search(string text)
    {
        ParsedVehicleKind kind = await _origin.Search(text);
        if (kind)
            _logger.Info("From db search vehicle kind: {0}. Parameter: {1}.", (string)kind, text);
        else
            _logger.Warn("Unable to db search vehicle kind. Parameter: {0}.", text);
        return kind;
    }
}