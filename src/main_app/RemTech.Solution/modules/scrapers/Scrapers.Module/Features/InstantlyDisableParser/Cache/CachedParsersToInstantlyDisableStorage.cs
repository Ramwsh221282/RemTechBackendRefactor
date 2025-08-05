using System.Text.Json;
using Scrapers.Module.Features.CreateNewParser.Cache;
using Scrapers.Module.Features.InstantlyDisableParser.Database;
using Scrapers.Module.Features.InstantlyDisableParser.Exceptions;
using Scrapers.Module.Features.InstantlyDisableParser.Models;
using StackExchange.Redis;

namespace Scrapers.Module.Features.InstantlyDisableParser.Cache;

internal sealed class CachedParsersToInstantlyDisableStorage(
    ConnectionMultiplexer multiplexer,
    IParsersToInstantlyDisableStorage origin
) : IParsersToInstantlyDisableStorage
{
    private const string ArrayKey = "parsers_array";
    private const string EntryKey = "parser_{0}_{1}";

    public Task<ParserToInstantlyDisable> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        return origin.Fetch(parserName, parserType, ct);
    }

    public async Task<InstantlyDisabledParser> Save(
        InstantlyDisabledParser parser,
        CancellationToken ct = default
    )
    {
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, parser.ParserName, parser.ParserType);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (string.IsNullOrEmpty(cachedJson) || arrayJson == null)
            throw new UnableToFindParserToInstantlyDisableException(
                parser.ParserName,
                parser.ParserType
            );
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (cached == null || array == null)
            throw new UnableToFindParserToInstantlyDisableException(
                parser.ParserName,
                parser.ParserType
            );
        cached = cached with { State = parser.ParserState };
        for (int i = 0; i < array.Length; i++)
        {
            CachedParser entry = array[i];
            if (entry.Name != parser.ParserName && entry.Type != parser.ParserType)
                continue;
            entry = entry with { State = parser.ParserState };
            array[i] = entry;
            break;
        }
        await db.StringSetAsync(cachedKey, JsonSerializer.Serialize(cached));
        await db.StringSetAsync(arrayJson, JsonSerializer.Serialize(array));
        return parser;
    }
}
