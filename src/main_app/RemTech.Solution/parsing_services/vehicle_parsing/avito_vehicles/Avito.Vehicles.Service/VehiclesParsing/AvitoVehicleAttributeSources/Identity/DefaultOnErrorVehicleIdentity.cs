using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;
using RemTech.Core.Shared.Primitives;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Identity;

public sealed class DefaultOnErrorVehicleIdentity : IParsedVehicleIdentitySource
{
    private readonly IParsedVehicleIdentitySource _origin;

    public DefaultOnErrorVehicleIdentity(IParsedVehicleIdentitySource origin)
    {
        _origin = origin;
    }

    public async Task<ParsedVehicleIdentity> Read()
    {
        try
        {
            return await _origin.Read();
        }
        catch
        {
            return new ParsedVehicleIdentity(new NotEmptyString(string.Empty));
        }
    }
}
