using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using RemTech.Logging.Library;

namespace Parsing.Vehicles.DbSearch.VehicleGeos;

public sealed class LoggingVehicleGeoDbSearch : IVehicleGeoDbSearch
{
    private readonly ICustomLogger _logger;
    private readonly IVehicleGeoDbSearch _search;

    public LoggingVehicleGeoDbSearch(ICustomLogger logger, IVehicleGeoDbSearch search)
    {
        _logger = logger;
        _search = search;
    }
    
    public async Task<ParsedVehicleGeo> Search(string text)
    {
        ParsedVehicleGeo geo = await  _search.Search(text);
        string region = geo.Region();
        string city = geo.City();
        if (geo)
            _logger.Info("Vehicle geo search result: Region - {0}. City - {1}. Parameter - {2}.", region, city, text);
        else
            _logger.Warn("Unable to search vehicle geo. Parameter: {0}.", text);
        return geo;
    }
}