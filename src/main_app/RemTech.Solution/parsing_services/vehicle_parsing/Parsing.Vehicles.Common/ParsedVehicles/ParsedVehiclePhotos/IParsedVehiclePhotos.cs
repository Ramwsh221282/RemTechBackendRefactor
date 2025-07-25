namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;

public interface IParsedVehiclePhotos
{
    Task<UniqueParsedVehiclePhotos> Read();
}