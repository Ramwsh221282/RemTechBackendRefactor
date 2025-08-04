namespace Scrapers.Module.Features.CreateNewParser.Cache;

internal sealed record CachedParserLink(
    string Name,
    string ParserName,
    string Url,
    bool Activity,
    int Processed,
    int TotalSeconds,
    int Hours,
    int Minutes,
    int Seconds
);
