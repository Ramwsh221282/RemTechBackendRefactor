using RemTech.Core.Shared.Result;

namespace RemTech.Core.Shared.Monads;

public sealed class ValidValues<TOne, TSecond>
{
    private readonly List<Error> _errors = [];
    public TOne Value { get; }
    public TSecond Second { get; }

    public ValidValues(TOne value, TSecond second)
    {
        Value = value;
        Second = second;
    }

    public ValidValues<TOne, TSecond> AddValidation(
        Func<TOne, bool> predicateOne,
        Error onFirstFailure,
        Func<TSecond, bool> predicateTwo,
        Error onSecondFailure)
    {
        if (!predicateOne(Value))
            _errors.Add(onFirstFailure);
        if (!predicateTwo(Second))
            _errors.Add(onSecondFailure);
        return this;
    }

    public Status<Unit> Validate()
    {
        if (_errors.Count > 0)
            Error.Combined(_errors);
        return Unit.Value;
    }
}

public sealed class ValidValue<TOne>
{
    private readonly List<Error> _errors = [];
    public TOne Value { get; }

    public ValidValue(TOne value)
    {
        Value = value;
    }

    public ValidValue<TOne> AddValidation(Func<TOne, bool> predicateOne, Error onFirstFailure)
    {
        if (!predicateOne(Value))
            _errors.Add(onFirstFailure);
        return this;
    }

    public Status<Unit> Validate()
    {
        if (_errors.Count > 0)
            Error.Combined(_errors);
        return Unit.Value;
    }
}