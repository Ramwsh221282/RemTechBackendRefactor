using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Serilog;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

public sealed class LoggingKindSource : IParsedVehicleKindSource
{
    private readonly ILogger _log;
    private readonly IParsedVehicleKindSource _origin;

    public LoggingKindSource(ILogger log, IParsedVehicleKindSource origin)
    {
        _log = log;
        _origin = origin;
    }
    
    public async Task<ParsedVehicleKind> Read()
    {
        ParsedVehicleKind kind = await _origin.Read();
        if (kind)
            _log.Information("Vehicle kind: {0}.", (string)kind);
        else
            _log.Warning("Unable to read vehicle kind.");
        return kind;
    }
}