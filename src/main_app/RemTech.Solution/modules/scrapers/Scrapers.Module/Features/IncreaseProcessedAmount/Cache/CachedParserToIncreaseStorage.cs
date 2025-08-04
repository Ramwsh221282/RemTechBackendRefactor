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
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, parser.ParserName, parser.ParserType);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (string.IsNullOrEmpty(cachedJson) || arrayJson == null)
            throw new UnableToFindIncreaseProcessedParserException(
                parser.ParserName,
                parser.ParserType,
                parser.ParserLinkName
            );
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (cached == null || array == null)
            throw new UnableToFindIncreaseProcessedParserException(
                parser.ParserName,
                parser.ParserType,
                parser.ParserLinkName
            );
        CachedParserLink[] cachedLinks = cached.Links.ToArray();
        for (int i = 0; i < cachedLinks.Length; i++)
        {
            CachedParserLink entry = cachedLinks[i];
            if (entry.Name != parser.ParserLinkName || entry.ParserName != parser.ParserName)
                continue;
            cachedLinks[i] = entry with { Processed = parser.LinkProcessed };
            break;
        }
        cached = cached with { Processed = parser.ParserProcessed, Links = cachedLinks };
        for (int i = 0; i < array.Length; i++)
        {
            CachedParser entry = array[i];
            if (entry.Name != parser.ParserName && entry.Type != parser.ParserType)
                continue;
            entry = entry with { Links = cachedLinks };
            array[i] = entry;
            break;
        }
        await db.StringSetAsync(cachedKey, JsonSerializer.Serialize(cached));
        await db.StringSetAsync(arrayJson, JsonSerializer.Serialize(array));
        return parser;
    }
}
