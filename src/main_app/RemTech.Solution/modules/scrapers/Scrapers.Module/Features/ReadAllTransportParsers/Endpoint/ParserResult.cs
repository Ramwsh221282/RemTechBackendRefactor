namespace Scrapers.Module.Features.ReadAllTransportParsers.Endpoint;

public sealed record ParserResult(
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
    HashSet<ParserLinkResult> Links
);
