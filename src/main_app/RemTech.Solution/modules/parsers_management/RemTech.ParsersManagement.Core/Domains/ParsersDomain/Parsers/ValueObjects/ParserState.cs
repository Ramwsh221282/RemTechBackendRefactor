using RemTech.ParsersManagement.Core.Common.Extensions;
using RemTech.ParsersManagement.Core.Common.Primitives;
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

    public static Status<ParserState> New(string? input)
    {
        Status<NotEmptyString> notEmpty = NotEmptyString.New(input);
        return notEmpty switch
        {
            { IsFailure: true } => Error.Validation("Строка состояния парсера была пустой."),
            { Value: { } val } when val.Length() > 150 => Error.Validation(
                "Строка состояния парсера больше 150 символов."
            ),
            { Value: { } val } when val.Length() < 5 => Error.Validation(
                "Строка состояния парсера меньше 5 символов."
            ),
            _ => New(notEmpty),
        };
    }

    public static implicit operator NotEmptyString(ParserState state) => state._value;

    public static implicit operator string(ParserState state) => state._value;
}
