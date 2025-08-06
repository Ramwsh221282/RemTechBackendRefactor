namespace Scrapers.Module.Features.CreateNewParser.Cache;

internal sealed record CachedParserLink(
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
