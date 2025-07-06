namespace RemTech.ParsersManagement.Core.Common.Primitives;

public sealed class Length
{
    private readonly int _value;

    public Length(NotEmptyStringLength nesLength) => _value = nesLength;

    public Length(NotEmptyString nes) => _value = new NotEmptyStringLength(nes);

    public Length(int scalar) => _value = scalar;

    public static implicit operator int(Length length) => length._value;
}
