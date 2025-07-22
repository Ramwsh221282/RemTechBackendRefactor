using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using RemTech.Core.Shared.Primitives;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Model;

public sealed class DefaultOnErrorModel : IParsedVehicleModelSource
{
    private readonly IParsedVehicleModelSource _origin;

    public DefaultOnErrorModel(IParsedVehicleModelSource origin)
    {
        _origin = origin;
    }
    
    public async Task<ParsedVehicleModel> Read()
    {
        try
        {
            return await _origin.Read();
        }
        catch
        {
            return new ParsedVehicleModel(new NotEmptyString(string.Empty));
        }
    }
}