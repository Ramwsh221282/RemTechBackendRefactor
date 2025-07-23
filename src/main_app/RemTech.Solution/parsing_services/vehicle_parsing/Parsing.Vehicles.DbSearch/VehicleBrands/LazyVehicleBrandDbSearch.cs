using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.DbSearch.VehicleBrands;

public sealed class LazyVehicleBrandDbSearch : IVehicleBrandDbSearch
{
    private readonly IVehicleBrandDbSearch _origin;

    public LazyVehicleBrandDbSearch(IVehicleBrandDbSearch origin)
    {
        _origin = origin;
    }

    public Task<ParsedVehicleBrand> Search(string text)
    {
        return string.IsNullOrWhiteSpace(text) 
            ? Task.FromResult(new ParsedVehicleBrand(new NotEmptyString(string.Empty)))
            : _origin.Search(text);
    }
}