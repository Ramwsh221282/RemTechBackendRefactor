using System.Text.Json;
using Scrapers.Module.Features.CreateNewParser.Cache;
using Scrapers.Module.Features.ReadAllTransportParsers.Endpoint;
using StackExchange.Redis;

namespace Scrapers.Module.Features.ReadAllTransportParsers.Storage;

internal sealed class CachedAllTransportParsersStorage(
    ConnectionMultiplexer multiplexer,
    IAllTransportParsersStorage storage
) : IAllTransportParsersStorage
{
    private const string ArrayKey = "parsers_array";

    public async Task<IEnumerable<ParserResult>> Read(CancellationToken ct = default)
    {
        IDatabase db = multiplexer.GetDatabase();
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (arrayJson == null)
            return await storage.Read(ct);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (array == null)
            return await storage.Read(ct);
        if (array.Length == 0)
            return await storage.Read(ct);
        return array
            .Select(p => new ParserResult(
                p.Name,
                p.Type,
                p.State,
                p.Domain,
                p.Processed,
                p.TotalSeconds,
                p.Hours,
                p.Minutes,
                p.Seconds,
                p.WaitDays,
                p.LastRun,
                p.NextRun,
                p.Links.Select(l => new ParserLinkResult(
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
            ))
            .OrderBy(p => p.Name);
    }
}
