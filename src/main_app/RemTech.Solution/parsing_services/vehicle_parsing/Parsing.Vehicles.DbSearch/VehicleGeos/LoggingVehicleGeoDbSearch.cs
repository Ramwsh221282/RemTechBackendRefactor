﻿using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using Serilog;

namespace Parsing.Vehicles.DbSearch.VehicleGeos;

public sealed class LoggingVehicleGeoDbSearch : IVehicleGeoDbSearch
{
    private readonly ILogger _logger;
    private readonly IVehicleGeoDbSearch _search;

    public LoggingVehicleGeoDbSearch(ILogger logger, IVehicleGeoDbSearch search)
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
            _logger.Information("Vehicle geo search result: Region - {0}. City - {1}. Parameter - {2}.", region, city, text);
        else
            _logger.Warning("Unable to search vehicle geo. Parameter: {0}.", text);
        return geo;
    }
}