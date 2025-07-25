namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;

public interface IParsedVehicleKindSource
{
    Task<ParsedVehicleKind> Read();
}