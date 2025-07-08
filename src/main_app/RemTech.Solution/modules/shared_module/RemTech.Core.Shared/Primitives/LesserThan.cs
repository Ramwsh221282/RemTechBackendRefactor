namespace RemTech.Core.Shared.Primitives;

public readonly record struct LesserThan
{
    private readonly bool _value = false;

    public LesserThan(Length length, Length related)
        : this((int)length, (int)related) { }

    public LesserThan(int scalar, int related)
    {
        _value = scalar < related;
    }

    public static implicit operator bool(LesserThan l) => l._value;
}
