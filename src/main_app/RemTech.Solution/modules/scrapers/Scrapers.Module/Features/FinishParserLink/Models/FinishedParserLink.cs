using Scrapers.Module.Features.FinishParserLink.Database;

namespace Scrapers.Module.Features.FinishParserLink.Models;

internal sealed record FinishedParserLink(
    string ParserName,
    string ParserType,
    string LinkName,
    long TotalElapsedSeconds,
    int Seconds,
    int Minutes,
    int Hours
)
{
    public async Task Save(IFinishedParserLinkStorage storage)
    {
        await storage.Save(this);
    }
}
