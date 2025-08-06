using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Brand;

public sealed class LoggingBrandSource : IParsedVehicleBrandSource
{
    private readonly Serilog.ILogger _log;
    private readonly IParsedVehicleBrandSource _origin;

    public LoggingBrandSource(Serilog.ILogger log, IParsedVehicleBrandSource origin)
    {
        _log = log;
        _origin = origin;
    }

    public async Task<ParsedVehicleBrand> Read()
    {
        ParsedVehicleBrand brand = await _origin.Read();
        if (brand)
            _log.Information("Vehicle brand: {0}.", (string)brand);
        else
            _log.Warning("Unable to read vehicle brand.");
        return brand;
    }
}
