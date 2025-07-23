namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;

public interface IVehicleCharacteristics
{
    public IEnumerable<VehicleCharacteristic> Read();
}