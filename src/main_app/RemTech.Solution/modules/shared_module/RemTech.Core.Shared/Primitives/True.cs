namespace RemTech.Core.Shared.Primitives;

public sealed class True
{
    private readonly bool _value;

    public True(bool value) => _value = value;

    public static implicit operator bool(True value) => value._value;
}
