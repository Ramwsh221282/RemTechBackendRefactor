using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using Serilog;

namespace Parsing.Vehicles.DbSearch.VehicleBrands;

public sealed class LoggingVehicleBrandDbSearch : IVehicleBrandDbSearch
{
    private readonly ILogger _logger;
    private readonly IVehicleBrandDbSearch _origin;

    public LoggingVehicleBrandDbSearch(ILogger logger, IVehicleBrandDbSearch origin)
    {
        _logger = logger;
        _origin = origin;
    }

    public async Task<ParsedVehicleBrand> Search(string text)
    {
        ParsedVehicleBrand brand = await _origin.Search(text);
        if (brand)
            _logger.Information("Db search vehicle brand: {0}. Parameter: {1}.", (string)brand, text);
        else
            _logger.Warning("Unable to search vehicle brand from Db. Parameter: {0}.", text);
        return brand;
    }
}