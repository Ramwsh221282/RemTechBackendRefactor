namespace Vehicles.Domain.VehicleContext.ValueObjects;

public sealed record VehicleDescription(string Value)
{
    public const int MaxLength = 500;
}
