using Scrapers.Module.Features.RemovingParserLink.Database;
using Scrapers.Module.Features.RemovingParserLink.Exceptions;
using Scrapers.Module.Features.RemovingParserLink.Models;

namespace Scrapers.Module.Features.RemovingParserLink.Logging;

internal sealed class LoggingRemovedParserLinkStorage(
    Serilog.ILogger logger,
    IRemovedParserLinksStorage origin
) : IRemovedParserLinksStorage
{
    public async Task<ParserLinkToRemove> Fetch(
        string linkName,
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        try
        {
            return await origin.Fetch(linkName, parserName, parserType, ct);
        }
        catch (ParserLinkToRemoveNotFoundException ex)
        {
            logger.Error("{Ex}", ex.Message);
            throw;
        }
    }

    public async Task<RemovedParserLink> Save(
        RemovedParserLink parserLink,
        CancellationToken ct = default
    )
    {
        try
        {
            RemovedParserLink removed = await origin.Save(parserLink, ct);
            logger.Information(
                "Ссылка {Name} {Pname} {Ptype} удалена.",
                removed.Name,
                removed.ParserName,
                removed.ParserType
            );
            return removed;
        }
        catch (ParserLinkToRemoveNotFoundException ex)
        {
            logger.Error("{Ex}", ex.Message);
            throw;
        }
    }
}
