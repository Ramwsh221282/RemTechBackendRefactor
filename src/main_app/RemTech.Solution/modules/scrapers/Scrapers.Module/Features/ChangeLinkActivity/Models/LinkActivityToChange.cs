using Scrapers.Module.Features.ChangeLinkActivity.Exceptions;

namespace Scrapers.Module.Features.ChangeLinkActivity.Models;

internal sealed record LinkActivityToChange(
    string Name,
    string ParserName,
    string ParserType,
    bool CurrentActivity,
    string ParserState
)
{
    public LinkWithChangedActivity Change(bool activity)
    {
        if (ParserState == "Работает")
            throw new UnableToChangeLinkActivityOfWorkingParserException();
        return CurrentActivity == activity
            ? throw new LinkActivitySameException(Name)
            : new LinkWithChangedActivity(Name, ParserName, ParserType, activity);
    }
}
