using System.Text.Json;
using Scrapers.Module.Features.CreateNewParser.Cache;
using Scrapers.Module.Features.RemovingParserLink.Database;
using Scrapers.Module.Features.RemovingParserLink.Exceptions;
using Scrapers.Module.Features.RemovingParserLink.Models;
using StackExchange.Redis;

namespace Scrapers.Module.Features.RemovingParserLink.Cache;

internal sealed class CachedLinksToRemoveStorage(
    ConnectionMultiplexer multiplexer,
    IRemovedParserLinksStorage origin
) : IRemovedParserLinksStorage
{
    private const string ArrayKey = "parsers_array";
    private const string EntryKey = "parser_{0}_{1}";

    public Task<ParserLinkToRemove> Fetch(
        string linkName,
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        return origin.Fetch(linkName, parserName, parserType, ct);
    }

    public async Task<RemovedParserLink> Save(
        RemovedParserLink parserLink,
        CancellationToken ct = default
    )
    {
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, parserLink.ParserName, parserLink.ParserType);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (string.IsNullOrEmpty(cachedJson) || arrayJson == null)
            throw new ParserLinkToRemoveNotFoundException(
                parserLink.Name,
                parserLink.ParserName,
                parserLink.ParserType
            );
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (cached == null || array == null)
            throw new ParserLinkToRemoveNotFoundException(
                parserLink.Name,
                parserLink.ParserName,
                parserLink.ParserType
            );

        CachedParserLink[] cachedLinks = cached
            .Links.Where(l => l.Name != parserLink.Name && l.ParserName != parserLink.ParserName)
            .ToArray();
        cached = cached with { Links = cachedLinks };
        for (int i = 0; i < array.Length; i++)
        {
            CachedParser entry = array[i];
            if (entry.Name != parserLink.ParserName && entry.Type != parserLink.ParserType)
                continue;
            entry = entry with { Links = cachedLinks };
            array[i] = entry;
            break;
        }
        await db.StringSetAsync(cachedKey, JsonSerializer.Serialize(cached));
        await db.StringSetAsync(arrayJson, JsonSerializer.Serialize(array));
        return parserLink;
    }
}
