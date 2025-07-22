namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;

public interface IParsedVehicleBrandSource
{
    Task<ParsedVehicleBrand> Read();
}