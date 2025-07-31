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
            try
            {
                IVehicleKindDbSearch search = _searches.Dequeue();
                ParsedVehicleKind kind = await search.Search(text);
                return kind;
            }
            catch (Exception ex)
            {
                continue;
            }
        }

        throw new ArgumentException("Unable to fetch vehicle kind from db");
    }
}
