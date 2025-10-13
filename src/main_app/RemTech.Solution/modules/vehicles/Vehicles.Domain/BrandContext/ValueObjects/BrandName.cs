namespace Vehicles.Domain.BrandContext.ValueObjects;

public sealed record BrandName(string Name)
{
    public const int MaxLength = 80;
}
