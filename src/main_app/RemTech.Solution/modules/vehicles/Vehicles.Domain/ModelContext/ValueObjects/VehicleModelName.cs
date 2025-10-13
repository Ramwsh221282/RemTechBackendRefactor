namespace Vehicles.Domain.ModelContext.ValueObjects;

public sealed record VehicleModelName(string Value)
{
    public const int MaxLength = 80;
}
