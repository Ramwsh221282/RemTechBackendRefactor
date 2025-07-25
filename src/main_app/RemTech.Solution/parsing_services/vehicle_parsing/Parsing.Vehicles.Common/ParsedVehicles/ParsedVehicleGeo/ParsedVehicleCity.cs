using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

public sealed class ParsedVehicleCity
{
    private readonly NotEmptyString _city;

    public ParsedVehicleCity(NotEmptyString city)
    {
        _city = city;
    }

    public ParsedVehicleCity(string city)
    {
        _city = new NotEmptyString(city);
    }

    public ParsedVehicleCity()
    {
        _city = new NotEmptyString(string.Empty);
    }

    public static implicit operator string(ParsedVehicleCity city) => city._city;
    public static implicit operator NotEmptyString(ParsedVehicleCity city) => city._city;
    public static implicit operator bool(ParsedVehicleCity city) => city._city;
}