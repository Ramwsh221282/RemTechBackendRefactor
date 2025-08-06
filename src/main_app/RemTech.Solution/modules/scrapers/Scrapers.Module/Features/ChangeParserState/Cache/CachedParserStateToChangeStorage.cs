using System.Text.Json;
using Scrapers.Module.Features.ChangeParserState.Database;
using Scrapers.Module.Features.ChangeParserState.Exception;
using Scrapers.Module.Features.ChangeParserState.Models;
using Scrapers.Module.Features.CreateNewParser.Cache;
using StackExchange.Redis;

namespace Scrapers.Module.Features.ChangeParserState.Cache;

internal sealed class CachedParserStateToChangeStorage(
    ConnectionMultiplexer multiplexer,
    IParserStateToChangeStorage origin
) : IParserStateToChangeStorage
{
    private const string ArrayKey = "parsers_array";
    private const string EntryKey = "parser_{0}_{1}";

    public Task<ParserStateToChange> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        return origin.Fetch(parserName, parserType, ct);
    }

    public async Task<ParserWithChangedState> Save(
        ParserWithChangedState parser,
        CancellationToken ct = default
    )
    {
        ParserWithChangedState changed = await origin.Save(parser, ct);
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, changed.ParserName, changed.ParserType);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (string.IsNullOrEmpty(cachedJson) || arrayJson == null)
            throw new ParserStateToChangeNotFoundException(changed.ParserName, changed.ParserType);
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (cached == null || array == null)
            throw new ParserStateToChangeNotFoundException(changed.ParserName, changed.ParserType);
        cached = cached with { State = changed.NewState };
        for (int i = 0; i < array.Length; i++)
        {
            CachedParser entry = array[i];
            if (entry.Name != changed.ParserName && entry.Type != changed.ParserType)
                continue;
            entry = entry with { State = changed.NewState };
            array[i] = entry;
            break;
        }
        await db.StringSetAsync(cachedKey, JsonSerializer.Serialize(cached));
        await db.StringSetAsync(arrayJson, JsonSerializer.Serialize(array));
        return changed;
    }
}
