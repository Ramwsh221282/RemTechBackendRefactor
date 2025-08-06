namespace Scrapers.Module.Features.StartParser.Models;

internal sealed record ParserToStart(
    string ParserName,
    string ParserType,
    string State,
    HashSet<ParserLinksToStart> Links
)
{
    public StartedParser Start()
    {
        HashSet<StartedParserLink> links = Links
            .Select(l => new StartedParserLink(l.LinkName, l.LinkUrl, l.LinkParserName))
            .ToHashSet();
        return new StartedParser(ParserName, ParserType, links);
    }
}
