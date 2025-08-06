using System.Text.Json;
using Scrapers.Module.Features.CreateNewParser.Cache;
using Scrapers.Module.Features.IncreaseProcessedAmount.Database;
using Scrapers.Module.Features.IncreaseProcessedAmount.Exceptions;
using Scrapers.Module.Features.IncreaseProcessedAmount.Models;
using StackExchange.Redis;

namespace Scrapers.Module.Features.IncreaseProcessedAmount.Cache;

internal sealed class CachedParserToIncreaseStorage(
    ConnectionMultiplexer multiplexer,
    IParserToIncreaseStorage storage
) : IParserToIncreaseStorage
{
    private const string ArrayKey = "parsers_array";
    private const string EntryKey = "parser_{0}_{1}";

    public Task<ParserToIncreaseProcessed> Fetch(
        string parserName,
        string parserType,
        string linkName,
        CancellationToken ct = default
    )
    {
        return storage.Fetch(parserName, parserType, linkName, ct);
    }

    public async Task<ParserWithIncreasedProcessed> Save(
        ParserWithIncreasedProcessed parser,
        CancellationToken ct = default
    )
    {
        ParserWithIncreasedProcessed increased = await storage.Save(parser, ct);
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, increased.ParserName, increased.ParserType);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (string.IsNullOrEmpty(cachedJson) || arrayJson == null)
            throw new UnableToFindIncreaseProcessedParserException(
                increased.ParserName,
                increased.ParserType,
                increased.ParserLinkName
            );
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (cached == null || array == null)
            throw new UnableToFindIncreaseProcessedParserException(
                increased.ParserName,
                increased.ParserType,
                increased.ParserLinkName
            );
        CachedParserLink[] cachedLinks = cached.Links.ToArray();
        for (int i = 0; i < cachedLinks.Length; i++)
        {
            CachedParserLink entry = cachedLinks[i];
            if (entry.Name != increased.ParserLinkName || entry.ParserName != increased.ParserName)
                continue;
            cachedLinks[i] = entry with { Processed = increased.LinkProcessed };
            break;
        }
        cached = cached with { Processed = increased.ParserProcessed, Links = cachedLinks };
        for (int i = 0; i < array.Length; i++)
        {
            CachedParser entry = array[i];
            if (entry.Name != increased.ParserName && entry.Type != increased.ParserType)
                continue;
            entry = entry with { Links = cachedLinks };
            array[i] = entry;
            break;
        }
        await db.StringSetAsync(cachedKey, JsonSerializer.Serialize(cached));
        await db.StringSetAsync(arrayJson, JsonSerializer.Serialize(array));
        return increased;
    }
}
