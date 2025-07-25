using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.DbSearch.VehicleModels;

public sealed class LazyVehicleModelDbSearch : IVehicleModelDbSearch
{
    private readonly IVehicleModelDbSearch _search;

    public LazyVehicleModelDbSearch(IVehicleModelDbSearch search)
    {
        _search = search;
    }

    public Task<ParsedVehicleModel> Search(string text)
    {
        return string.IsNullOrWhiteSpace(text) 
            ? Task.FromResult(new ParsedVehicleModel(new NotEmptyString(string.Empty)))
            :  _search.Search(text);
    }
}