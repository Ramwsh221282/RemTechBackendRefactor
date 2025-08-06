using Scrapers.Module.Features.ChangeParserState.Models;

namespace Scrapers.Module.Features.ChangeParserState.Database;

internal interface IParserStateToChangeStorage
{
    Task<ParserStateToChange> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    );
    Task<ParserWithChangedState> Save(
        ParserWithChangedState parser,
        CancellationToken ct = default
    );
}
