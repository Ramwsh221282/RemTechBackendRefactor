using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Geos;

public sealed class LoggingVehicleGeoSource : IParsedVehicleGeoSource
{
    private readonly ICustomLogger _logger;
    private readonly IParsedVehicleGeoSource _origin;

    public LoggingVehicleGeoSource(ICustomLogger logger, IParsedVehicleGeoSource origin)
    {
        _logger = logger;
        _origin = origin;
    }

    public async Task<ParsedVehicleGeo> Read()
    {
        ParsedVehicleGeo geo = await _origin.Read();
        if (geo)
            _logger.Info("Vehicle geo info: Region - {0}. City - {1}.", (string)geo.Region(),  (string)geo.City());
        else
            _logger.Warn("Unable to read vehicle geo.");
        return geo;
    }
}