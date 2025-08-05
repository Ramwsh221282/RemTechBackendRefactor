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
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, parser.ParserName, parser.ParserType);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (string.IsNullOrEmpty(cachedJson) || arrayJson == null)
            throw new CannotFindParserToFinishException(parser.ParserName, parser.ParserType);
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (cached == null || array == null)
            throw new CannotFindParserToFinishException(parser.ParserName, parser.ParserType);
        cached = cached with
        {
            TotalSeconds = parser.TotalElapsedSeconds,
            Hours = parser.Hours,
            Minutes = parser.Minutes,
            Seconds = parser.Seconds,
        };
        for (int i = 0; i < array.Length; i++)
        {
            CachedParser entry = array[i];
            if (entry.Name != parser.ParserName && entry.Type != parser.ParserType)
                continue;
            entry = entry with
            {
                TotalSeconds = parser.TotalElapsedSeconds,
                Hours = parser.Hours,
                Minutes = parser.Minutes,
                Seconds = parser.Seconds,
            };
            array[i] = entry;
            break;
        }
        await db.StringSetAsync(cachedKey, JsonSerializer.Serialize(cached));
        await db.StringSetAsync(arrayJson, JsonSerializer.Serialize(array));
        return parser;
    }
}
