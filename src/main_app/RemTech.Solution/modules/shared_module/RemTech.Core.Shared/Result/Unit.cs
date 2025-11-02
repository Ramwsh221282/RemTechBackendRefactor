namespace RemTech.Core.Shared.Result;

public sealed class Unit
{
    public static readonly Unit Value = new();

    public static Unit Return(Action action)
    {
        action();
        return Value;
    }

    private Unit()
    {
    }
}