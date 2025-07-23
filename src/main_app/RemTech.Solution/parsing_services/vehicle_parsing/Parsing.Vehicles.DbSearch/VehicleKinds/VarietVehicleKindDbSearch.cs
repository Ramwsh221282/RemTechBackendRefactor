using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.DbSearch.VehicleKinds;

public sealed class VarietVehicleKindDbSearch : IVehicleKindDbSearch
{
    private readonly Queue<IVehicleKindDbSearch> _searches = [];

    public VarietVehicleKindDbSearch With(IVehicleKindDbSearch search)
    {
        _searches.Enqueue(search);
        return this;
    }
    
    public async Task<ParsedVehicleKind> Search(string text)
    {
        while (_searches.Count > 0)
        {
            IVehicleKindDbSearch search = _searches.Dequeue();
            ParsedVehicleKind kind = await search.Search(text);
            if (kind)
                return kind;
        }

        return new ParsedVehicleKind(new NotEmptyString(string.Empty));
    }
}