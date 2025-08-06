namespace Scrapers.Module.Features.FinishParser.Models;

internal sealed record FinishedParser(
    string ParserName,
    string ParserType,
    long TotalElapsedSeconds,
    int Seconds,
    int Hours,
    int Minutes
);
