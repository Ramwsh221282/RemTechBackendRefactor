using Scrapers.Module.Features.ChangeLinkActivity.Models;

namespace Scrapers.Module.Features.ChangeLinkActivity.Database;

internal interface ILinkActivityToChangeStorage
{
    Task<LinkActivityToChange> Fetch(
        string name,
        string parserName,
        string parserType,
        CancellationToken ct = default
    );
    Task<LinkWithChangedActivity> Save(
        LinkWithChangedActivity link,
        CancellationToken ct = default
    );
}
