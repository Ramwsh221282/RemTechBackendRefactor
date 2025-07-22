using Parsing.SDK.Logging;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

public sealed class LoggingKindSource : IParsedVehicleKindSource
{
    private readonly IParsingLog _log;
    private readonly IParsedVehicleKindSource _origin;

    public LoggingKindSource(IParsingLog log, IParsedVehicleKindSource origin)
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
            _log.Warning("Unable to read vehicle kind.");
        return kind;
    }
}