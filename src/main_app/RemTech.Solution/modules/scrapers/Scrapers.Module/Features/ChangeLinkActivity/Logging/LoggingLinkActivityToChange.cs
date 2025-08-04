using Scrapers.Module.Features.ChangeLinkActivity.Database;
using Scrapers.Module.Features.ChangeLinkActivity.Exceptions;
using Scrapers.Module.Features.ChangeLinkActivity.Models;
using Serilog;

namespace Scrapers.Module.Features.ChangeLinkActivity.Logging;

internal sealed class LoggingLinkActivityToChange(
    ILogger logger,
    ILinkActivityToChangeStorage storage
) : ILinkActivityToChangeStorage
{
    public async Task<LinkActivityToChange> Fetch(
        string name,
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        try
        {
            return await storage.Fetch(name, parserName, parserType, ct);
        }
        catch (LinkActivityToChangeNotFoundException ex)
        {
            logger.Error("{Ex}.", ex.Message);
            throw;
        }
    }

    public async Task<LinkWithChangedActivity> Save(
        LinkWithChangedActivity link,
        CancellationToken ct = default
    )
    {
        try
        {
            LinkWithChangedActivity changed = await storage.Save(link, ct);
            logger.Information(
                "Changed link activity. {Name}. {Activity}.",
                changed.Name,
                changed.CurrentActivity
            );
            return changed;
        }
        catch (UnableToChangeLinkActivityOfWorkingParserException ex)
        {
            logger.Error("{Ex}.", ex.Message);
            throw;
        }
    }
}
