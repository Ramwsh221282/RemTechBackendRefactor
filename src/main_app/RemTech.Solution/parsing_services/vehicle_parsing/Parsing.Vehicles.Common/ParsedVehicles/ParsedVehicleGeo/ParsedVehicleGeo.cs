namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

public sealed class ParsedVehicleGeo
{
    private readonly ParsedVehicleRegion _region;
    private readonly ParsedVehicleCity _city;

    public ParsedVehicleGeo(ParsedVehicleRegion region, ParsedVehicleCity city)
    {
        _region = region;
        _city = city;
    }

    public ParsedVehicleRegion Region() => _region;
    public ParsedVehicleCity City() => _city;

    public static implicit operator bool(ParsedVehicleGeo geo)
    {
        return geo._region;
    }
}