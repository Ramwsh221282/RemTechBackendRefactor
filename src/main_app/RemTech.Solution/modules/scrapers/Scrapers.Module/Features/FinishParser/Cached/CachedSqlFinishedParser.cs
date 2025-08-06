using System.Text.Json;
using Scrapers.Module.Features.CreateNewParser.Cache;
using Scrapers.Module.Features.FinishParser.Database;
using Scrapers.Module.Features.FinishParser.Exceptions;
using Scrapers.Module.Features.FinishParser.Models;
using StackExchange.Redis;

namespace Scrapers.Module.Features.FinishParser.Cached;

internal sealed class CachedSqlFinishedParser(
    ConnectionMultiplexer multiplexer,
    IParserToFinishStorage storage
) : IParserToFinishStorage
{
    private const string ArrayKey = "parsers_array";
    private const string EntryKey = "parser_{0}_{1}";

    public Task<ParserToFinish> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        return storage.Fetch(parserName, parserType, ct);
    }

    public async Task<FinishedParser> Save(FinishedParser parser, CancellationToken ct = default)
    {
        FinishedParser finished = await storage.Save(parser, ct);
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, finished.ParserName, finished.ParserType);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (string.IsNullOrEmpty(cachedJson) || arrayJson == null)
            throw new CannotFindParserToFinishException(finished.ParserName, finished.ParserType);
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (cached == null || array == null)
            throw new CannotFindParserToFinishException(finished.ParserName, finished.ParserType);
        cached = cached with
        {
            TotalSeconds = finished.TotalElapsedSeconds,
            Hours = finished.Hours,
            Minutes = finished.Minutes,
            Seconds = finished.Seconds,
        };
        for (int i = 0; i < array.Length; i++)
        {
            CachedParser entry = array[i];
            if (entry.Name != finished.ParserName && entry.Type != finished.ParserType)
                continue;
            entry = entry with
            {
                TotalSeconds = finished.TotalElapsedSeconds,
                Hours = finished.Hours,
                Minutes = finished.Minutes,
                Seconds = finished.Seconds,
            };
            array[i] = entry;
            break;
        }
        await db.StringSetAsync(cachedKey, JsonSerializer.Serialize(cached));
        await db.StringSetAsync(arrayJson, JsonSerializer.Serialize(array));
        return finished;
    }
}
