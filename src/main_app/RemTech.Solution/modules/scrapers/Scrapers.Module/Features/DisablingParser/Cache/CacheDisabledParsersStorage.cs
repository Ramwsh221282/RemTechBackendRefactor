using System.Text.Json;
using Scrapers.Module.Features.CreateNewParser.Cache;
using Scrapers.Module.Features.DisablingParser.Models;
using StackExchange.Redis;

namespace Scrapers.Module.Features.DisablingParser.Database;

internal sealed class CacheDisabledParsersStorage(
    ConnectionMultiplexer multiplexer,
    IDisabledParsersStorage origin
) : IDisabledParsersStorage
{
    private const string ArrayKey = "parsers_array";
    private const string EntryKey = "parser_{0}";

    public async Task<DisabledParser> SaveAsync(
        DisabledParser parser,
        CancellationToken ct = default
    )
    {
        DisabledParser disabled = await origin.SaveAsync(parser, ct);
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, disabled.Name);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (string.IsNullOrEmpty(cachedJson) || arrayJson == null)
            throw new UnableToFindParserToDisableException();
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (cached == null || array == null)
            throw new UnableToFindParserToDisableException();
        cached = cached with { State = disabled.State };
        for (int i = 0; i < array.Length; i++)
        {
            CachedParser entry = array[i];
            if (entry.Name != disabled.Name)
                continue;
            entry = entry with { State = disabled.State };
            array[i] = entry;
            break;
        }
        await db.StringSetAsync(cachedKey, JsonSerializer.Serialize(cached));
        await db.StringSetAsync(arrayJson, JsonSerializer.Serialize(array));
        return disabled;
    }
}
