using RemTech.Core.Shared.Extensions;
using RemTech.Core.Shared.Primitives;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

public sealed class ParserState
{
    public static ParserState Waiting() => new(NotEmptyString.New("Ожидает"));

    public static ParserState Working() => new(NotEmptyString.New("В процессе"));

    public static ParserState Disabled() => new(NotEmptyString.New("Выключен"));

    private static readonly ParserState[] _allowed = [Waiting(), Working(), Disabled()];

    private readonly NotEmptyString _value;

    private ParserState(NotEmptyString value) => _value = value;

    public ParserState StartWaiting() => Waiting();

    public ParserState StartWorking() => Working();

    public ParserState Disable() => Disabled();

    public bool AtWork() => _value.Same(Working());

    public bool AtDisabled() => _value.Same(Disabled());

    public bool AtEnabled() => _value.Same(Waiting());

    public bool IsState(NotEmptyString value) => _allowed.Maybe(s => s._value.Same(value)).Any();

    public NotEmptyString Read() => _value;

    public static Status<ParserState> New(NotEmptyString value)
    {
        ParserState? allowed = _allowed.FirstOrDefault(st => st._value.Same(value));
        return allowed == null
            ? new Error(
                $"Недопустимое состояние парсера: {value.StringValue()}",
                ErrorCodes.Validation
            )
            : allowed;
    }

    public static Status<ParserState> New(string? input) =>
        new SwitchReturn<NotEmptyString, Status<ParserState>>(new NotEmptyString(input))
            .With(
                comp => new True(new LengthIs(new Length(comp), 0)),
                _ => Error.Conflict("Строка состояния парсера была пустой.")
            )
            .With(
                comp => new True(new GreaterThan(new Length(comp), new Length(150))),
                _ => Error.Validation("Строка состояния парсера была пустой.")
            )
            .With(
                comp => new True(new LesserThan(new Length(comp), new Length(5))),
                _ => Error.Validation("Строка состояния парсера меньше 5 символов.")
            )
            .Default(nes => new ParserState(nes))
            .Return();

    public static implicit operator NotEmptyString(ParserState state) => state._value;

    public static implicit operator string(ParserState state) => state._value;

    public override bool Equals(object? obj) =>
        obj switch
        {
            null => false,
            ParserState ps => ps._value.Equals(_value),
            _ => false,
        };

    public override int GetHashCode() => _value.GetHashCode();
}
