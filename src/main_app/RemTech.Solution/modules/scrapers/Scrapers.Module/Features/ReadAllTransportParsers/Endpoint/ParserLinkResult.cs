namespace Scrapers.Module.Features.ReadAllTransportParsers.Endpoint;

public sealed record ParserLinkResult(
    string Name,
    string ParserName,
    string ParserType,
    string Url,
    bool Activity,
    int Processed,
    long TotalSeconds,
    int Hours,
    int Minutes,
    int Seconds
);
