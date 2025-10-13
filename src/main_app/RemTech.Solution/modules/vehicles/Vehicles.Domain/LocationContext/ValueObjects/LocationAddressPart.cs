namespace Vehicles.Domain.LocationContext.ValueObjects;

public sealed record LocationAddressPart(string Value)
{
    public const int MaxLength = 50;
}
