namespace RemTech.Core.Shared.Primitives;

public readonly record struct True
{
    private readonly bool _value = true;

    public True(bool value) => _value = value;

    public static implicit operator bool(True value) => value._value;
}
