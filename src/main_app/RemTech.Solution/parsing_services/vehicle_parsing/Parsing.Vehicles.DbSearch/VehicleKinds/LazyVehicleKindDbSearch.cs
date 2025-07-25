using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.DbSearch.VehicleKinds;

public sealed class LazyVehicleKindDbSearch : IVehicleKindDbSearch
{
    private readonly IVehicleKindDbSearch _origin;

    public LazyVehicleKindDbSearch(IVehicleKindDbSearch origin)
    {
        _origin = origin;
    }
    
    public Task<ParsedVehicleKind> Search(string text)
    {
        return string.IsNullOrWhiteSpace(text) 
            ? Task.FromResult(new ParsedVehicleKind(new NotEmptyString(string.Empty))) 
            : _origin.Search(text);
    }
}