namespace Scrapers.Module.Features.StartParser.Models;

internal sealed record StartedParser(
    string ParserName,
    string ParserType,
    HashSet<StartedParserLink> Links
);
