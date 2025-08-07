using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Identity;

public sealed class LoggingVehicleIdentity : IParsedVehicleIdentitySource
{
    private readonly Serilog.ILogger _log;
    private readonly IParsedVehicleIdentitySource _origin;

    public LoggingVehicleIdentity(Serilog.ILogger log, IParsedVehicleIdentitySource origin)
    {
        _log = log;
        _origin = origin;
    }

    public async Task<ParsedVehicleIdentity> Read()
    {
        ParsedVehicleIdentity identity = await _origin.Read();
        if (identity)
            _log.Information("Vehicle ID: {0}.", (string)identity);
        else
            _log.Warning("Unable to read vehicle ID");
        return identity;
    }
}
