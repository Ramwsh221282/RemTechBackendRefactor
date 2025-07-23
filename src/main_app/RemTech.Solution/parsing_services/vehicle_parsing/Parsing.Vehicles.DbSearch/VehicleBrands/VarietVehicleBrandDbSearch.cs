using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.DbSearch.VehicleBrands;

public sealed class VarietVehicleBrandDbSearch : IVehicleBrandDbSearch
{
    private readonly Queue<IVehicleBrandDbSearch> _searches = [];

    public VarietVehicleBrandDbSearch With(IVehicleBrandDbSearch dbSearch)
    {
        _searches.Enqueue(dbSearch);
        return this;
    }

    public async Task<ParsedVehicleBrand> Search(string text)
    {
        while (_searches.Count > 0)
        {
            IVehicleBrandDbSearch dbSearch = _searches.Dequeue();
            ParsedVehicleBrand brand = await dbSearch.Search(text);
            if (brand)
                return brand;
        }

        return new ParsedVehicleBrand(new NotEmptyString(string.Empty));
    }
}