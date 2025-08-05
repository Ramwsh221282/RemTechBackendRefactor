namespace Scrapers.Module.Features.ChangeParserState.Models;

internal sealed record ParserWithChangedState(
    string ParserName,
    string ParserType,
    string NewState
);
