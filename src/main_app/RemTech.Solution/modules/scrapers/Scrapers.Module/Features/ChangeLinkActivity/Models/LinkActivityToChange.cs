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
    public LinkWithChangedActivity Change()
    {
        if (ParserState == "Работает")
            throw new UnableToChangeLinkActivityOfWorkingParserException();
        return new LinkWithChangedActivity(Name, ParserName, ParserType, CurrentActivity);
    }
}
