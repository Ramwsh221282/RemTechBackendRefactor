namespace Parsing.Vehicles.Common.ParsedVehicles;

public interface IParsedVehicleSource
{
    IAsyncEnumerable<IParsedVehicle> Iterate();
}