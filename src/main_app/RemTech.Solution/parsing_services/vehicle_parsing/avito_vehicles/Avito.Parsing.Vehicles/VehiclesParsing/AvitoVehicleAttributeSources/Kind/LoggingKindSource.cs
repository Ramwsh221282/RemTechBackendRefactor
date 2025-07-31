using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using RemTech.Core.Shared.Primitives;
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
        try
        {
            ParsedVehicleKind kind = await _origin.Read();
            _log.Information("Vehicle kind: {0}.", (string)kind);
            return kind;
        }
        catch
        {
            _log.Warning("Returning default kind.");
            return new ParsedVehicleKind(new NotEmptyString(string.Empty));
        }
    }
}
