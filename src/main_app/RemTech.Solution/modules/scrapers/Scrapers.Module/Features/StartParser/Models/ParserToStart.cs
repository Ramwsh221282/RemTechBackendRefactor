namespace Scrapers.Module.Features.StartParser.Models;

public sealed record ParserToStart(
    string ParserName,
    string ParserType,
    string ParserDomain,
    HashSet<ParserLinksToStart> Links
)
{
    public StartedParser Start()
    {
        HashSet<StartedParserLink> links = Links
            .Select(l => new StartedParserLink(l.LinkName, l.LinkUrl, l.LinkParserName))
            .ToHashSet();
        string startedState = "Работает";
        return new StartedParser(ParserName, ParserType, ParserDomain, startedState, 0, links);
    }
}
