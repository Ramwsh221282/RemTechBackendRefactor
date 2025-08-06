using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Geos;

public sealed class LoggingVehicleGeoSource : IParsedVehicleGeoSource
{
    private readonly Serilog.ILogger _logger;
    private readonly IParsedVehicleGeoSource _origin;

    public LoggingVehicleGeoSource(Serilog.ILogger logger, IParsedVehicleGeoSource origin)
    {
        _logger = logger;
        _origin = origin;
    }

    public async Task<ParsedVehicleGeo> Read()
    {
        ParsedVehicleGeo geo = await _origin.Read();
        if (geo)
            _logger.Information(
                "Vehicle geo info: Region - {0}. City - {1}.",
                (string)geo.Region(),
                (string)geo.City()
            );
        else
            _logger.Warning("Unable to read vehicle geo.");
        return geo;
    }
}
