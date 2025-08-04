using System.Text.Json;
using Scrapers.Module.Features.CreateNewParser.Cache;
using Scrapers.Module.Features.UpdateWaitDays.Database;
using Scrapers.Module.Features.UpdateWaitDays.Endpoint;
using Scrapers.Module.Features.UpdateWaitDays.Exceptions;
using StackExchange.Redis;

namespace Scrapers.Module.Features.UpdateWaitDays.Cache;

internal sealed class CacheParserWaitDaysToUpdateStorage(
    ConnectionMultiplexer multiplexer,
    IParserWaitDaysToUpdateStorage storage
) : IParserWaitDaysToUpdateStorage
{
    private const string ArrayKey = "parsers_array";
    private const string EntryKey = "parser_{0}_{1}";

    public Task<ParserWaitDaysToUpdate> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        return storage.Fetch(parserName, parserType, ct);
    }

    public async Task<ParserWithUpdatedWaitDays> Save(
        ParserWithUpdatedWaitDays parser,
        CancellationToken ct = default
    )
    {
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, parser.ParserName, parser.ParserType);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (string.IsNullOrEmpty(cachedJson) || arrayJson == null)
            throw new ParserToUpdateWaitDaysNotFoundException(parser.ParserName, parser.ParserType);
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (cached == null || array == null)
            throw new ParserToUpdateWaitDaysNotFoundException(parser.ParserName, parser.ParserType);
        cached = cached with { NextRun = parser.NextRun, WaitDays = parser.WaitDays };
        for (int i = 0; i < array.Length; i++)
        {
            CachedParser entry = array[i];
            if (entry.Name != parser.ParserName && entry.Type != parser.ParserType)
                continue;
            entry = entry with { NextRun = parser.NextRun, WaitDays = parser.WaitDays };
            array[i] = entry;
            break;
        }
        await db.StringSetAsync(cachedKey, JsonSerializer.Serialize(cached));
        await db.StringSetAsync(arrayJson, JsonSerializer.Serialize(array));
        return parser;
    }
}
