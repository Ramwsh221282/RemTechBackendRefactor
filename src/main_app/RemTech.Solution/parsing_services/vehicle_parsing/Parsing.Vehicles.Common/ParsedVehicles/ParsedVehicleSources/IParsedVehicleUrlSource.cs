namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;

public interface IParsedVehicleUrlSource
{
    Task<ParsedVehicleUrl> Read();
}