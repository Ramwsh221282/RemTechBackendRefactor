using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.DbSearch.VehicleModels;

public sealed class VarietVehicleModelDbSearch : IVehicleModelDbSearch
{
    private readonly Queue<IVehicleModelDbSearch> _searches = [];

    public VarietVehicleModelDbSearch With(IVehicleModelDbSearch search)
    {
        _searches.Enqueue(search);
        return this;
    }

    public async Task<ParsedVehicleModel> Search(string text)
    {
        while (_searches.Count > 0)
        {
            IVehicleModelDbSearch search = _searches.Dequeue();
            ParsedVehicleModel model = await search.Search(text);
            if (model)
                return model;
        }

        return new ParsedVehicleModel(new NotEmptyString(string.Empty));
    }
}