namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;

public interface IParsedVehiclePriceSource
{
    Task<ParsedVehiclePrice> Read();
}