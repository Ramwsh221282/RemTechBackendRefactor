namespace Scrapers.Module.Features.StartParser.Models;

public sealed record ParserLinksToStart(
    string LinkName,
    string LinkUrl,
    string LinkParserName,
    string LinkParserType
);
