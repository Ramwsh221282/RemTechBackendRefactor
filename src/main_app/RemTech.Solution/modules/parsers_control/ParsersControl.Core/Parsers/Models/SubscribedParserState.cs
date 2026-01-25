using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

public sealed record SubscribedParserState
{
    private static readonly string _sleeping = "Ожидает";
    private static readonly string _working = "В работе";
    private static readonly string _disabled = "Отключен";
    private static readonly string[] _states = [_sleeping, _working, _disabled];

    public string Value { get; }

    private SubscribedParserState(string value) => Value = value;

    public bool HasSameState(SubscribedParserState other) => Value == other.Value;

    public bool IsWorking() => Value == _working;

    public bool IsDisabled() => Value == _disabled;

    public bool IsSleeping() => Value == _sleeping;

    public static SubscribedParserState Sleeping => new(_sleeping);
    public static SubscribedParserState Working => new(_working);
    public static SubscribedParserState Disabled => new(_disabled);

    public static Result<SubscribedParserState> FromString(string input)
    {
        string? state = _states.FirstOrDefault(s => s == input);
        return state == null ? (Result<SubscribedParserState>)Error.InvalidFormat("Состояние работы парсера") : (Result<SubscribedParserState>)new SubscribedParserState(state);
    }
}
