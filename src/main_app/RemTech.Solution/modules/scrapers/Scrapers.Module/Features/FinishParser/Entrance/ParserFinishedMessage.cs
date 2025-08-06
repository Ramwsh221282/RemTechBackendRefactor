namespace Scrapers.Module.Features.FinishParser.Entrance;

internal sealed record ParserFinishedMessage(
    string ParserName,
    string ParserType,
    long TotalElapsedSeconds
);
