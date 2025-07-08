namespace RemTech.Core.Shared.Primitives;

public readonly record struct Length
{
    private readonly int _value = 0;

    public Length(NotEmptyStringLength nesLength) => _value = nesLength;

    public Length(NotEmptyString nes) => _value = new NotEmptyStringLength(nes);

    public Length(int scalar) => _value = scalar;

    public static implicit operator int(Length length) => length._value;
}
