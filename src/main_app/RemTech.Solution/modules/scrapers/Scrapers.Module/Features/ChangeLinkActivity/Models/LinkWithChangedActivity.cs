namespace Scrapers.Module.Features.ChangeLinkActivity.Models;

internal sealed record LinkWithChangedActivity(
    string Name,
    string ParserName,
    string ParserType,
    bool CurrentActivity
);
