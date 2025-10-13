using Vehicles.Domain.VehicleContext.ValueObjects;

namespace Vehicles.UseCases.AddVehicle;

public sealed record AddVehicleCommandCharacteristic(string Name, string Value)
{
    public VehicleCharacteristic ProvideCharacteristic() => new VehicleCharacteristic(Name, Value);
}
