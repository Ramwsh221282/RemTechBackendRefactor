namespace Scrapers.Module.Features.FinishParserLink.Entrance;

internal sealed record FinishedParserLinkMessage(
    string ParserName,
    string ParserType,
    string LinkName,
    long TotalElapsedSeconds
);
