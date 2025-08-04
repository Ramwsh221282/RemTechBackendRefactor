using Scrapers.Module.Features.CreateNewParserLink.Database;
using Scrapers.Module.Features.CreateNewParserLink.Exceptions;
using Scrapers.Module.Features.CreateNewParserLink.Models;

namespace Scrapers.Module.Features.CreateNewParserLink.Logging;

internal sealed class LoggingParsersWithLinkStorage(
    Serilog.ILogger logger,
    IParsersWithNewLinkStorage origin
) : IParsersWithNewLinkStorage
{
    public async Task<ParserWithNewLink> Save(
        ParserWithNewLink parser,
        CancellationToken ct = default
    )
    {
        try
        {
            ParserWithNewLink saved = await origin.Save(parser, ct);
            logger.Information(
                "Saved parser link: {Name} {ParserName}",
                saved.Link.Name,
                saved.Link.ParserName
            );
            return saved;
        }
        catch (CannotPutLinkToWorkingParserException ex)
        {
            logger.Error("{Ex}", ex.Message);
            throw;
        }
        catch (ParserLinkAlreadyExistsInParserException ex)
        {
            logger.Error("{Ex}", ex.Message);
            throw;
        }
        catch (ParserLinkUrlAlreadyExistsException ex)
        {
            logger.Error("{Ex}", ex.Message);
            throw;
        }
        catch (ParserWhereToPutLinkNameEmptyException ex)
        {
            logger.Error("{Ex}", ex.Message);
            throw;
        }
        catch (ParserWhereToPutLinkUnsupportedTypeException ex)
        {
            logger.Error("{Ex}", ex.Message);
            throw;
        }
        catch (UnkownParserStateException ex)
        {
            logger.Error("{Ex}", ex.Message);
            throw;
        }
    }
}
