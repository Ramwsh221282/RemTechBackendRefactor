using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

namespace Parsing.Vehicles.DbSearch.VehicleGeos;

public sealed class LazyVehicleGeoDbSearch : IVehicleGeoDbSearch
{
    private readonly IVehicleGeoDbSearch _search;

    public LazyVehicleGeoDbSearch(IVehicleGeoDbSearch search)
    {
        _search = search;
    }
    
    public Task<ParsedVehicleGeo> Search(string text)
    {
        return string.IsNullOrWhiteSpace(text) 
            ? Task.FromResult(new ParsedVehicleGeo(new ParsedVehicleRegion(), new ParsedVehicleCity()))
            :  _search.Search(text);
    }
}