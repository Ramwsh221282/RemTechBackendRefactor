using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;
using RemTech.Core.Shared.Primitives;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Url;

public sealed class DefaultOnErrorVehicleUrl : IParsedVehicleUrlSource
{
    private readonly IParsedVehicleUrlSource _origin;

    public DefaultOnErrorVehicleUrl(IParsedVehicleUrlSource origin)
    {
        _origin = origin;
    }
    
    public async Task<ParsedVehicleUrl> Read()
    {
        try
        {
            return await _origin.Read();
        }
        catch
        {
            return new ParsedVehicleUrl(new NotEmptyString(string.Empty));
        }
    }
}