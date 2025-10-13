namespace Vehicles.Domain.CategoryContext.ValueObjects;

public sealed record CategoryName(string Value)
{
    public const int MaxLength = 80;
}
