using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Price;

public sealed class LoggingVehiclePriceSource : IParsedVehiclePriceSource
{
    private readonly ICustomLogger _log;
    private readonly IParsedVehiclePriceSource _origin;

    public LoggingVehiclePriceSource(ICustomLogger log, IParsedVehiclePriceSource origin)
    {
        _log = log;
        _origin = origin;
    }
    
    public async Task<ParsedVehiclePrice> Read()
    {
        _log.Info("Reading avito vehicle price.");
        ParsedVehiclePrice price = await _origin.Read();
        
        if (price)
            _log.Info("Vehicle price value: {0}. Vehicle price NDS: {1}.", (long)price, price.IsNds());
        else
            _log.Warn("Unable to read vehicle price.");
        
        return price;
    }
}