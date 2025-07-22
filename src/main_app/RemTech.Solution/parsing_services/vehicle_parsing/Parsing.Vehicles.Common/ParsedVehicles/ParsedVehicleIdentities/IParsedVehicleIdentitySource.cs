namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;

public interface IParsedVehicleIdentitySource
{
    Task<ParsedVehicleIdentity> Read();
}