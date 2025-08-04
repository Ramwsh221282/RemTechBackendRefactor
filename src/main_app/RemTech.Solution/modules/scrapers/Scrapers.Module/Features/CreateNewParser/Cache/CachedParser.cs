namespace Scrapers.Module.Features.CreateNewParser.Cache;

internal sealed record CachedParser(
    string Name,
    string Type,
    string State,
    string Domain,
    int Processed,
    long TotalSeconds,
    int Hours,
    int Minutes,
    int Seconds,
    int WaitDays,
    DateTime LastRun,
    DateTime NextRun,
    IEnumerable<CachedParserLink> Links
);
