using System.Text.Json;
using Scrapers.Module.Features.CreateNewParser.Extensions;
using Scrapers.Module.Features.CreateNewParser.Models;
using Serilog;
using StackExchange.Redis;

namespace Scrapers.Module.Features.CreateNewParser.Cache;

internal sealed class LoggingCachedNewParsers(ILogger logger, ICachedNewParsers origin)
    : ICachedNewParsers
{
    public async Task Save(NewParser parser, CancellationToken ct = default)
    {
        try
        {
            await origin.Save(parser, ct);
            logger.Information(
                "Cached parser {Name} {Type} {Domain}",
                parser.Name,
                parser.Type.Type,
                parser.Domain.Domain
            );
        }
        catch (Exception ex)
        {
            logger.Fatal("Failure at caching parser. {Exception}", ex.Message);
        }
    }
}

internal sealed class CachedNewParsers(ConnectionMultiplexer multiplexer) : ICachedNewParsers
{
    private const string ArrayKey = "parsers_array";
    private const string EntryKey = "parser_{0}_{1}";

    public async Task Save(NewParser parser, CancellationToken ct = default)
    {
        CachedParser cached = parser.Cached();
        string serialized = cached.Serialized();
        IDatabase database = multiplexer.GetDatabase();
        await database.StringSetAsync(
            string.Format(EntryKey, cached.Name, cached.Type),
            serialized
        );
        string? array = await database.StringGetAsync(ArrayKey);
        if (string.IsNullOrWhiteSpace(array))
            await SaveNewArray(cached, database);
        else
            await UpdateArray(array, cached, database);
    }

    private static async Task SaveNewArray(CachedParser parser, IDatabase database)
    {
        CachedParser[] array = [parser];
        string serialized = JsonSerializer.Serialize(array);
        await database.StringSetAsync(ArrayKey, serialized);
    }

    private static async Task UpdateArray(string array, CachedParser parser, IDatabase database)
    {
        CachedParser[]? deserialized = JsonSerializer.Deserialize<CachedParser[]>(ArrayKey);
        if (deserialized == null)
            throw new ApplicationException("Deserialized parser cached array is null.");
        for (int i = 0; i < deserialized.Length; i++)
        {
            CachedParser entry = deserialized[i];
            if (entry.Name != parser.Name)
                continue;
            deserialized[i] = parser;
            break;
        }
        await database.StringSetAsync(ArrayKey, JsonSerializer.Serialize(deserialized));
    }
}
