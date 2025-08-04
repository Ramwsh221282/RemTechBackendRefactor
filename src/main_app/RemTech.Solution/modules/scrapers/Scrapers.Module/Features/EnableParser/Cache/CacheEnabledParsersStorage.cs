using System.Text.Json;
using Scrapers.Module.Features.CreateNewParser.Cache;
using Scrapers.Module.Features.EnableParser.Database;
using Scrapers.Module.Features.EnableParser.Exceptions;
using Scrapers.Module.Features.EnableParser.Models;
using StackExchange.Redis;

namespace Scrapers.Module.Features.EnableParser.Cache;

internal sealed class CacheEnabledParsersStorage(
    ConnectionMultiplexer multiplexer,
    IEnabledParsersStorage origin
) : IEnabledParsersStorage
{
    private const string ArrayKey = "parsers_array";
    private const string EntryKey = "parser_{0}_{1}";

    public async Task<EnabledParser> Save(
        EnabledParser parser,
        CancellationToken cancellationToken = default
    )
    {
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, parser.Name, parser.Type);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (string.IsNullOrEmpty(cachedJson) || arrayJson == null)
            throw new ParserToEnableWasNotFoundException(parser);
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (cached == null || array == null)
            throw new ParserToEnableWasNotFoundException(parser);
        cached = cached with { State = parser.State };
        for (int i = 0; i < array.Length; i++)
        {
            CachedParser entry = array[i];
            if (entry.Name != parser.Name && entry.Type != parser.Type)
                continue;
            entry = entry with { State = parser.State };
            array[i] = entry;
            break;
        }
        await db.StringSetAsync(cachedKey, JsonSerializer.Serialize(cached));
        await db.StringSetAsync(arrayJson, JsonSerializer.Serialize(array));
        return parser;
    }
}
