namespace RemTech.Core.Shared.Primitives;

public readonly record struct GreaterThan
{
    private readonly bool _value = false;

    public GreaterThan(Length length, int related)
        : this((int)length, related) { }

    public GreaterThan(NotEmptyStringLength length, int related)
        : this((int)length, related) { }

    public GreaterThan(Length length, Length related)
    {
        int lengthScalar = length;
        int relatedScalar = related;
        _value = lengthScalar > relatedScalar;
    }

    public GreaterThan(int scalar, int related) => _value = scalar > related;

    public static implicit operator bool(GreaterThan g) => g._value;
}
