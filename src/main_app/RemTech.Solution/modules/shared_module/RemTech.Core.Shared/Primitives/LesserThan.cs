namespace RemTech.Core.Shared.Primitives;

public sealed class LesserThan
{
    private readonly bool _value;

    public LesserThan(Length length, Length related)
        : this((int)length, (int)related) { }

    public LesserThan(int scalar, int related)
    {
        _value = scalar < related;
    }

    public static implicit operator bool(LesserThan l) => l._value;
}
