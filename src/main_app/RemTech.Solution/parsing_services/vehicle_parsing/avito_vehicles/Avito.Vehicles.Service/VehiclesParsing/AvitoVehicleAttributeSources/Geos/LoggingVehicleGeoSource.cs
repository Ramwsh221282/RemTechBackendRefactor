using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Geos;

public sealed class LoggingVehicleGeoSource(Serilog.ILogger logger, IParsedVehicleGeoSource origin)
    : IParsedVehicleGeoSource
{
    public async Task<ParsedVehicleGeo> Read()
    {
        ParsedVehicleGeo geo = await origin.Read();
        if (geo)
            logger.Information("Vehicle geo info: Region - {0}.", (string)geo);
        else
            logger.Warning("Unable to read vehicle geo.");
        return geo;
    }
}
