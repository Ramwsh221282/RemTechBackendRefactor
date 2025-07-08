namespace RemTech.Core.Shared.Primitives;

public sealed class LengthIs
{
    private readonly bool _value;

    public LengthIs(Length length, Length related)
        : this((int)length, (int)related) { }

    public LengthIs(Length length, int related)
        : this((int)length, related) { }

    public LengthIs(NotEmptyStringLength nesLength, int related)
        : this((int)nesLength, related) { }

    public LengthIs(NotEmptyString nes, int related)
        : this(new NotEmptyStringLength(nes), related) { }

    public LengthIs(int scalar, int related) => _value = scalar == related;

    public static implicit operator bool(LengthIs lis) => lis._value;
}
