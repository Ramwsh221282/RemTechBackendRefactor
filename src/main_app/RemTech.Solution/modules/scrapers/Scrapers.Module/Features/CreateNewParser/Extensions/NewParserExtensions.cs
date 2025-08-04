using System.Text.Json;
using Scrapers.Module.Features.CreateNewParser.Cache;
using Scrapers.Module.Features.CreateNewParser.Database;
using Scrapers.Module.Features.CreateNewParser.Models;

namespace Scrapers.Module.Features.CreateNewParser.Extensions;

internal static class NewParserExtensions
{
    internal static async Task Store(
        this NewParser parser,
        INewParsersStorage storage,
        CancellationToken ct = default
    ) => await storage.Save(parser, ct);

    internal static async Task Store(
        this NewParser parser,
        ICachedNewParsers cached,
        CancellationToken ct = default
    ) => await cached.Save(parser, ct);

    internal static CachedParser Cached(this NewParser parser)
    {
        return new CachedParser(
            parser.Name,
            parser.Type.Type,
            parser.State.State,
            parser.Domain.Domain,
            parser.Statistics.ProcessedAmount,
            parser.Statistics.TotalElapsedSeconds,
            parser.Statistics.ElapsedHours,
            parser.Statistics.ElapsedMinutes,
            parser.Statistics.ElapsedSeconds,
            parser.Schedule.WaitDays,
            parser.Schedule.LastRun,
            parser.Schedule.NextRun,
            []
        );
    }

    internal static string Serialized(this CachedParser parser)
    {
        return JsonSerializer.Serialize(parser);
    }

    internal static CachedParser Deserialized(this string parserJson)
    {
        CachedParser? parser = JsonSerializer.Deserialize<CachedParser>(parserJson);
        return parser
            ?? throw new ApplicationException("Parser deserialization caused in null parser.");
    }
}
