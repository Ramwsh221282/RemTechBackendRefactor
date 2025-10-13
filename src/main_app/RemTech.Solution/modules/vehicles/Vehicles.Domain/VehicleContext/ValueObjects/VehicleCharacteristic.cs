namespace Vehicles.Domain.VehicleContext.ValueObjects;

public sealed record VehicleCharacteristic(string Name, string Value)
{
    public const int MaxLength = 100;
}
