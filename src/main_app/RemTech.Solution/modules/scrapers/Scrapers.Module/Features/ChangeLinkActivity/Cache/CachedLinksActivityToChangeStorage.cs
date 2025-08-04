using System.Text.Json;
using Scrapers.Module.Features.ChangeLinkActivity.Database;
using Scrapers.Module.Features.ChangeLinkActivity.Exceptions;
using Scrapers.Module.Features.ChangeLinkActivity.Models;
using Scrapers.Module.Features.CreateNewParser.Cache;
using StackExchange.Redis;

namespace Scrapers.Module.Features.ChangeLinkActivity.Cache;

internal sealed class CachedLinksActivityToChangeStorage(
    ConnectionMultiplexer multiplexer,
    ILinkActivityToChangeStorage storage
) : ILinkActivityToChangeStorage
{
    private const string ArrayKey = "parsers_array";
    private const string EntryKey = "parser_{0}_{1}";

    public async Task<LinkActivityToChange> Fetch(
        string name,
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        return await storage.Fetch(name, parserName, parserType, ct);
    }

    public async Task<LinkWithChangedActivity> Save(
        LinkWithChangedActivity link,
        CancellationToken ct = default
    )
    {
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, link.ParserName, link.ParserType);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (string.IsNullOrEmpty(cachedJson) || arrayJson == null)
            throw new LinkActivityToChangeNotFoundException(
                link.Name,
                link.ParserName,
                link.ParserType
            );
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (cached == null || array == null)
            throw new LinkActivityToChangeNotFoundException(
                link.Name,
                link.ParserName,
                link.ParserType
            );
        CachedParserLink[] cachedLinks = cached.Links.ToArray();
        for (int i = 0; i < cachedLinks.Length; i++)
        {
            if (cachedLinks[i].Name == link.Name && cachedLinks[i].ParserName == link.ParserName)
                cachedLinks[i] = cachedLinks[i] with { Activity = link.CurrentActivity };
        }
        cached = cached with { Links = cachedLinks };
        for (int i = 0; i < array.Length; i++)
        {
            CachedParser entry = array[i];
            if (entry.Name != link.ParserName && entry.Type != link.ParserType)
                continue;
            entry = entry with { Links = cachedLinks };
            array[i] = entry;
            break;
        }
        await db.StringSetAsync(cachedKey, JsonSerializer.Serialize(cached));
        await db.StringSetAsync(arrayJson, JsonSerializer.Serialize(array));
        return link;
    }
}
