using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;
using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Identity;

public sealed class LoggingVehicleIdentity : IParsedVehicleIdentitySource
{
    private readonly ICustomLogger _log;
    private readonly IParsedVehicleIdentitySource _origin;

    public LoggingVehicleIdentity(ICustomLogger log, IParsedVehicleIdentitySource origin)
    {
        _log = log;
        _origin = origin;
    }
    
    public async Task<ParsedVehicleIdentity> Read()
    {
        ParsedVehicleIdentity identity = await _origin.Read();
        if (identity)
            _log.Info("Vehicle ID: {0}.", (string)identity);
        else
            _log.Warn("Unable to read vehicle ID");
        return identity;
    }
}