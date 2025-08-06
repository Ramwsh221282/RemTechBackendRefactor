using Scrapers.Module.Features.InstantlyDisableParser.Exceptions;

namespace Scrapers.Module.Features.InstantlyDisableParser.Models;

internal sealed record ParserToInstantlyDisable(
    string ParserName,
    string ParserType,
    string ParserState
)
{
    public InstantlyDisabledParser Disable()
    {
        return ParserState != "Работает"
            ? throw new CannotInstantlyDisableNotWorkingParserException(ParserName, ParserType)
            : new InstantlyDisabledParser(ParserName, ParserType, "Отключен");
    }
}
