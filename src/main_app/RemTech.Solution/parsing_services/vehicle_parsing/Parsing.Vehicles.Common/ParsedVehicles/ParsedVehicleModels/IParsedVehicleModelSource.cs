namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;

public interface IParsedVehicleModelSource
{
    Task<ParsedVehicleModel> Read();
}