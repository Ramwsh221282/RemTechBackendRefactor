namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

public interface IParsedVehicleGeoSource
{
    Task<ParsedVehicleGeo> Read();
}
