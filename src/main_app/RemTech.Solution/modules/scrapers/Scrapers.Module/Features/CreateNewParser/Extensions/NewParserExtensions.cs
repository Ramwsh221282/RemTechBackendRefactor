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
}
