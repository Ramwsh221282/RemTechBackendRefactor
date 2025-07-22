using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;

public sealed class ParsedVehicleBrand
{
    private readonly NotEmptyString _brand;

    public ParsedVehicleBrand(NotEmptyString brand) => _brand = brand;

    public ParsedVehicleBrand(string? brand) : this(new  NotEmptyString(brand))
    { }

    public static implicit operator NotEmptyString(ParsedVehicleBrand brand) => brand._brand;
    public static implicit operator string(ParsedVehicleBrand brand) => brand._brand;
    public static implicit operator bool(ParsedVehicleBrand brand) => brand._brand;
}