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
        ParserWithUpdatedWaitDays withUpdated = await storage.Save(parser, ct);
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, withUpdated.ParserName, withUpdated.ParserType);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (string.IsNullOrEmpty(cachedJson) || arrayJson == null)
            throw new ParserToUpdateWaitDaysNotFoundException(
                withUpdated.ParserName,
                withUpdated.ParserType
            );
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (cached == null || array == null)
            throw new ParserToUpdateWaitDaysNotFoundException(
                withUpdated.ParserName,
                withUpdated.ParserType
            );
        cached = cached with { NextRun = withUpdated.NextRun, WaitDays = withUpdated.WaitDays };
        for (int i = 0; i < array.Length; i++)
        {
            CachedParser entry = array[i];
            if (entry.Name != withUpdated.ParserName && entry.Type != withUpdated.ParserType)
                continue;
            entry = entry with { NextRun = withUpdated.NextRun, WaitDays = withUpdated.WaitDays };
            array[i] = entry;
            break;
        }
        await db.StringSetAsync(cachedKey, JsonSerializer.Serialize(cached));
        await db.StringSetAsync(arrayJson, JsonSerializer.Serialize(array));
        return withUpdated;
    }
}
