using Scrapers.Module.Features.ChangeParserState.Exception;

namespace Scrapers.Module.Features.ChangeParserState.Models;

internal sealed record ParserStateToChange(
    string ParserName,
    string ParserType,
    string CurrentState
)
{
    public ParserWithChangedState Change(bool stateValue)
    {
        string newState = stateValue switch
        {
            true => "Ожидает",
            false => "Отключен",
        };
        return CurrentState == newState
            ? throw new ParserAlreadyHasThisStateException(ParserName, ParserType, newState)
            : new ParserWithChangedState(ParserName, ParserType, newState);
    }
}
