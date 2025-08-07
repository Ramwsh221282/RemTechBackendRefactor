namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

public sealed class ParsedVehicleGeo
{
    private readonly string _region;

    public ParsedVehicleGeo(string? region)
    {
        _region = region ?? string.Empty;
    }

    public static implicit operator string(ParsedVehicleGeo region) => region._region;

    public static implicit operator bool(ParsedVehicleGeo region) =>
        !string.IsNullOrWhiteSpace(region._region);
}
