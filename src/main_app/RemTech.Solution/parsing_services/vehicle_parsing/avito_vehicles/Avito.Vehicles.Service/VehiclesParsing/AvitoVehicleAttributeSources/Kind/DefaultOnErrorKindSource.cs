using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using RemTech.Core.Shared.Primitives;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

public sealed class DefaultOnErrorKindSource : IParsedVehicleKindSource
{
    private readonly IParsedVehicleKindSource _origin;

    public DefaultOnErrorKindSource(IParsedVehicleKindSource origin)
    {
        _origin = origin;
    }

    public async Task<ParsedVehicleKind> Read()
    {
        try
        {
            return await _origin.Read();
        }
        catch
        {
            return new ParsedVehicleKind(new NotEmptyString(string.Empty));
        }
    }
}
