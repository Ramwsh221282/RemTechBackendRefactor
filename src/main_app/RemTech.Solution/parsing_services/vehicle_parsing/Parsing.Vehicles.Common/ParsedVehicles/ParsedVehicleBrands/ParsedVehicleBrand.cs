namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;

public sealed class ParsedVehicleBrand(string? brand)
{
    private readonly string _brand = brand ?? string.Empty;

    public static implicit operator string(ParsedVehicleBrand brand) => brand._brand;

    public static implicit operator bool(ParsedVehicleBrand brand) =>
        !string.IsNullOrWhiteSpace(brand._brand);
}
