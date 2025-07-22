using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using RemTech.Core.Shared.Primitives;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Brand;

public sealed class DefaultOnErrorBrandSource : IParsedVehicleBrandSource
{
    private readonly IParsedVehicleBrandSource _origin;

    public DefaultOnErrorBrandSource(IParsedVehicleBrandSource origin)
    {
        _origin = origin;
    }
    
    public async Task<ParsedVehicleBrand> Read()
    {
        try
        {
            return await _origin.Read();
        }
        catch
        {
            return new ParsedVehicleBrand(new NotEmptyString(string.Empty));
        }
    }
}