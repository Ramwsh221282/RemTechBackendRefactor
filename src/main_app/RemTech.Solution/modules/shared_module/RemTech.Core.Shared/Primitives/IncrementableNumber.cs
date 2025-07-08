namespace RemTech.Core.Shared.Primitives;

public readonly record struct IncrementableNumber
{
    private readonly PositiveInteger _number = new();

    public IncrementableNumber(PositiveInteger number) => _number = number;

    public IncrementableNumber() => _number = PositiveInteger.New();

    public IncrementableNumber Increase()
    {
        int next = _number.Read() + 1;
        return new IncrementableNumber(PositiveInteger.New(next));
    }

    public IncrementableNumber Reset() => new(PositiveInteger.New());

    public PositiveInteger Read() => _number;

    public static implicit operator int(IncrementableNumber number) => number._number;
}
