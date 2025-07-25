using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

namespace Parsing.Vehicles.DbSearch.VehicleGeos;

public sealed class VarietGeoDbSearch : IVehicleGeoDbSearch
{
    private readonly Queue<IVehicleGeoDbSearch> _searches = [];

    public VarietGeoDbSearch With(IVehicleGeoDbSearch searches)
    {
        _searches.Enqueue(searches);
        return this;
    }

    public async Task<ParsedVehicleGeo> Search(string text)
    {
        while (_searches.Count > 0)
        {
            IVehicleGeoDbSearch search = _searches.Dequeue();
            ParsedVehicleGeo geo = await search.Search(text);
            if (geo)
                return geo;
        }

        return new ParsedVehicleGeo(new ParsedVehicleRegion(), new ParsedVehicleCity());
    }
}