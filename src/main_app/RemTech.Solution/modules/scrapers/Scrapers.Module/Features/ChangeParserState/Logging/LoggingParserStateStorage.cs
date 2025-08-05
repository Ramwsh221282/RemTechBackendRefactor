using Scrapers.Module.Features.ChangeParserState.Database;
using Scrapers.Module.Features.ChangeParserState.Exception;
using Scrapers.Module.Features.ChangeParserState.Models;

namespace Scrapers.Module.Features.ChangeParserState.Logging;

internal sealed class LoggingParserStateStorage(
    Serilog.ILogger logger,
    IParserStateToChangeStorage storage
) : IParserStateToChangeStorage
{
    public async Task<ParserStateToChange> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        try
        {
            return await storage.Fetch(parserName, parserType, ct);
        }
        catch (ParserStateToChangeNotFoundException ex)
        {
            logger.Error("{Ex}.", ex.Message);
            throw;
        }
    }

    public async Task<ParserWithChangedState> Save(
        ParserWithChangedState parser,
        CancellationToken ct = default
    )
    {
        try
        {
            ParserWithChangedState changed = await storage.Save(parser, ct);
            logger.Information(
                "Парсер {Name} {Type} состояние изменено на {State}.",
                parser.ParserName,
                parser.ParserType,
                parser.NewState
            );
            return changed;
        }
        catch (ParserStateToChangeNotFoundException ex)
        {
            logger.Error("{Ex}.", ex.Message);
            throw;
        }
    }
}
