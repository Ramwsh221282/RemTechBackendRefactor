namespace RemTech.Functional.Extensions;

public sealed class Unit
{
    public static Unit Value { get; } = new();

    internal Unit()
    {
    }
}