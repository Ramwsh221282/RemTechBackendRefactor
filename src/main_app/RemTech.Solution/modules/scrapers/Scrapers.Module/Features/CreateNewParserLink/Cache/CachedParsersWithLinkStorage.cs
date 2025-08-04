using System.Text.Json;
using Scrapers.Module.Features.CreateNewParser.Cache;
using Scrapers.Module.Features.CreateNewParserLink.Database;
using Scrapers.Module.Features.CreateNewParserLink.Exceptions;
using Scrapers.Module.Features.CreateNewParserLink.Models;
using StackExchange.Redis;

namespace Scrapers.Module.Features.CreateNewParserLink.Cache;

internal sealed class CachedParsersWithLinkStorage(
    ConnectionMultiplexer multiplexer,
    IParsersWithNewLinkStorage storage
) : IParsersWithNewLinkStorage
{
    private const string ArrayKey = "parsers_array";
    private const string EntryKey = "parser_{0}";

    public async Task<ParserWithNewLink> Save(
        ParserWithNewLink parser,
        CancellationToken ct = default
    )
    {
        IDatabase db = multiplexer.GetDatabase();
        string cachedKey = string.Format(EntryKey, parser.Link.ParserName);
        string? cachedJson = await db.StringGetAsync(cachedKey);
        string? arrayJson = await db.StringGetAsync(ArrayKey);
        if (string.IsNullOrEmpty(cachedJson) || arrayJson == null)
            throw new ParserWhereToPutLinkNotFoundException(parser.Parser.Name, parser.Parser.Type);
        CachedParser? cached = JsonSerializer.Deserialize<CachedParser>(cachedJson);
        CachedParser[]? array = JsonSerializer.Deserialize<CachedParser[]>(arrayJson);
        if (cached == null || array == null)
            throw new ParserWhereToPutLinkNotFoundException(parser.Parser.Name, parser.Parser.Type);
        cached = cached with { Links = [.. cached.Links, CreateLink(parser)] };
        for (int i = 0; i < array.Length; i++)
        {
            CachedParser entry = array[i];
            if (entry.Name != parser.Link.ParserName)
                continue;
            entry = entry with { Links = cached.Links };
            array[i] = entry;
            break;
        }
        await db.StringSetAsync(cachedKey, JsonSerializer.Serialize(cached));
        await db.StringSetAsync(arrayJson, JsonSerializer.Serialize(array));
        return parser;
    }

    private CachedParserLink CreateLink(ParserWithNewLink parser)
    {
        return new CachedParserLink(
            parser.Link.Name,
            parser.Link.ParserName,
            parser.Link.Url,
            parser.Link.Active,
            parser.Link.Statistics.ParsedAmount,
            parser.Link.Statistics.TotalElapsedSeconds,
            parser.Link.Statistics.ElapsedHours,
            parser.Link.Statistics.ElapsedMinutes,
            parser.Link.Statistics.ElapsedSeconds
        );
    }
}
