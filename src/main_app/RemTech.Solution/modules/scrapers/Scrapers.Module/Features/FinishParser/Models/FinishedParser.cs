using Scrapers.Module.Features.FinishParser.Database;

namespace Scrapers.Module.Features.FinishParser.Models;

internal sealed record FinishedParser(
    string ParserName,
    string ParserType,
    DateTime LastRun,
    DateTime NextRun,
    long TotalElapsedSeconds,
    int Seconds,
    int Hours,
    int Minutes
)
{
    public Task Save(IParserToFinishStorage storage, CancellationToken ct = default)
    {
        return storage.Save(this, ct);
    }
}
