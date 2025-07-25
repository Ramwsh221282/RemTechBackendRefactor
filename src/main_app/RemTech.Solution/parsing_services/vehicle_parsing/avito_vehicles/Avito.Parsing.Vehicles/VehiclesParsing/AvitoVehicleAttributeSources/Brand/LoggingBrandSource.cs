using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Brand;

public sealed class LoggingBrandSource : IParsedVehicleBrandSource
{
    private readonly ICustomLogger _log;
    private readonly IParsedVehicleBrandSource _origin;

    public LoggingBrandSource(ICustomLogger log, IParsedVehicleBrandSource origin)
    {
        _log = log; 
        _origin = origin;
    }
    
    public async Task<ParsedVehicleBrand> Read()
    {
        ParsedVehicleBrand brand = await _origin.Read();
        if (brand)
            _log.Info("Vehicle brand: {0}.", (string)brand);
        else
            _log.Warn("Unable to read vehicle brand.");
        return brand;
    }
}