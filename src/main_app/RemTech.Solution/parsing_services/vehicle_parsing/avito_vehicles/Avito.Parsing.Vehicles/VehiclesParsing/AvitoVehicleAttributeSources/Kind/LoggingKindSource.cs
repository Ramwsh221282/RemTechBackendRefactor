using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

public sealed class LoggingKindSource : IParsedVehicleKindSource
{
    private readonly ICustomLogger _log;
    private readonly IParsedVehicleKindSource _origin;

    public LoggingKindSource(ICustomLogger log, IParsedVehicleKindSource origin)
    {
        _log = log;
        _origin = origin;
    }
    
    public async Task<ParsedVehicleKind> Read()
    {
        ParsedVehicleKind kind = await _origin.Read();
        if (kind)
            _log.Info("Vehicle kind: {0}.", (string)kind);
        else
            _log.Warn("Unable to read vehicle kind.");
        return kind;
    }
}