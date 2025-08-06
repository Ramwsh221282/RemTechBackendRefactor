using System.Text.Json;
using Scrapers.Module.Features.CreateNewParser.Cache;
using Scrapers.Module.Features.FinishParserLink.Database;
using Scrapers.Module.Features.FinishParserLink.Exceptions;
using Scrapers.Module.Features.FinishParserLink.Models;
using StackExchange.Redis;

namespace Scrapers.Module.Features.FinishParserLink.Cache;

internal sealed class CachedFinishedParserLinkStorage(
    ConnectionMultiplexer multiplexer,
    IFinishedParserLinkStorage origin
) : IFinishedParserLinkStorage
{
    private const string ArrayKey = "parsers_array";
    private const string EntryKey = "parser_{0}_{1}";

    public async Task<ParserLinkToFinish> Fetch(
        string parserName,
        string linkName,
        string parserType,
        CancellationToken ct = default
    )
    {
        return await origin.Fetch(parserName, linkName, parserType, ct);
    }

    public async Task<FinishedParserLink> Save(
        FinishedParserLink link,
        CancellationToken ct = default
    )
    {
        FinishedParserLink finished = await origin.Save(link, ct);
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, finished.ParserName, finished.ParserType);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (string.IsNullOrEmpty(cachedJson) || arrayJson == null)
            throw new ParserLinkToFinishNotFoundException(
                finished.ParserName,
                finished.LinkName,
                finished.ParserType
            );
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (cached == null || array == null)
            throw new ParserLinkToFinishNotFoundException(
                finished.ParserName,
                finished.LinkName,
                finished.ParserType
            );
        CachedParserLink[] links = cached.Links.ToArray();
        for (int i = 0; i < links.Length; i++)
        {
            CachedParserLink entry = links[i];
            if (entry.ParserName != finished.ParserName || entry.Name != finished.LinkName)
                continue;
            entry = entry with
            {
                TotalSeconds = finished.TotalElapsedSeconds,
                Hours = finished.Hours,
                Minutes = finished.Minutes,
                Seconds = finished.Seconds,
            };
            links[i] = entry;
            break;
        }

        cached = cached with { Links = links };
        for (int i = 0; i < array.Length; i++)
        {
            CachedParser entry = array[i];
            if (entry.Name != finished.ParserName && entry.Type != finished.ParserType)
                continue;
            entry = entry with { Links = links };
            array[i] = entry;
            break;
        }
        await db.StringSetAsync(cachedKey, JsonSerializer.Serialize(cached));
        await db.StringSetAsync(arrayJson, JsonSerializer.Serialize(array));
        return finished;
    }
}
