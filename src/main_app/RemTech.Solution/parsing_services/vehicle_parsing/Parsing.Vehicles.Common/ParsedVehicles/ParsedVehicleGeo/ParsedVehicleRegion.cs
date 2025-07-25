using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

public interface IParsedVehicleGeoSource
{
    Task<ParsedVehicleGeo> Read();
}

public sealed class ParsedVehicleRegion
{
    private readonly NotEmptyString _region;

    public ParsedVehicleRegion(NotEmptyString region)
    {
        _region = region;
    }

    public ParsedVehicleRegion(string region)
    {
        _region = new NotEmptyString(region);
    }

    public ParsedVehicleRegion()
    {
        _region = new NotEmptyString(string.Empty);
    }

    public static implicit operator string(ParsedVehicleRegion region) => region._region;
    public static implicit operator NotEmptyString(ParsedVehicleRegion region) => region._region;
    public static implicit operator bool(ParsedVehicleRegion region) => region._region;
}