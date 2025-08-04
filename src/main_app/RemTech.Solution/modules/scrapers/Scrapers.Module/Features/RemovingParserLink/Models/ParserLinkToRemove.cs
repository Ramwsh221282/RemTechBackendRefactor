using Scrapers.Module.Features.RemovingParserLink.Exceptions;

namespace Scrapers.Module.Features.RemovingParserLink.Models;

internal sealed record ParserLinkToRemove(
    string LinkName,
    string ParserName,
    string ParserType,
    string State
)
{
    public RemovedParserLink Remove()
    {
        if (State == "Работает")
            throw new UnableToRemoveParserLinkWhenWorkingException();
        return new RemovedParserLink(LinkName, ParserName, ParserType);
    }
}
