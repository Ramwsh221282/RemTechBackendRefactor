using Scrapers.Module.Features.RemovingParserLink.Exceptions;

namespace Scrapers.Module.Features.RemovingParserLink.Models;

internal sealed record ParserLinkToRemove(
    string LinkName,
    string ParserName,
    string ParserType,
    string State,
    string Url
)
{
    public RemovedParserLink Remove()
    {
        return State == "Работает"
            ? throw new UnableToRemoveParserLinkWhenWorkingException()
            : new RemovedParserLink(LinkName, ParserName, ParserType, Url);
    }
}
