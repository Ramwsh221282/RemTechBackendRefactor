using System.Text.Json;
using Scrapers.Module.Features.CreateNewParser.Cache;
using Scrapers.Module.Features.ReadAllTransportParsers.Endpoint;
using Scrapers.Module.Features.ReadConcreteScraper.Storage;
using StackExchange.Redis;

namespace Scrapers.Module.Features.ReadConcreteScraper.Cache;

internal sealed class CachedConcreteScraperStorage(
    ConnectionMultiplexer multiplexer,
    IConcreteScraperStorage origin
) : IConcreteScraperStorage
{
    private const string EntryKey = "parser_{0}_{1}";

    public async Task<ParserResult?> Read(string name, string type, CancellationToken ct = default)
    {
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, name, type);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        if (string.IsNullOrWhiteSpace(cachedJson))
            return await origin.Read(name, type, ct);
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        if (cached == null)
            return await origin.Read(name, type, ct);

        return new ParserResult(
            cached.Name,
            cached.Type,
            cached.State,
            cached.Domain,
            cached.Processed,
            cached.TotalSeconds,
            cached.Hours,
            cached.Minutes,
            cached.Seconds,
            cached.WaitDays,
            cached.LastRun,
            cached.NextRun,
            cached
                .Links.Select(l => new ParserLinkResult(
                    l.Name,
                    l.ParserName,
                    l.ParserType,
                    l.Url,
                    l.Activity,
                    l.Processed,
                    l.TotalSeconds,
                    l.Hours,
                    l.Minutes,
                    l.Seconds
                ))
                .ToHashSet()
        );
    }
}
