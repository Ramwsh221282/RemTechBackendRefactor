using Scrapers.Module.Domain.JournalsContext.Features.CompleteScraperJournal;
using Scrapers.Module.Features.FinishParser.Database;

namespace Scrapers.Module.Features.FinishParser.Models;

internal sealed record FinishedParser(
    string ParserName,
    string ParserType,
    string ParserState,
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

    public CompleteScraperJournalCommand CompleteJournalCommand()
    {
        return new CompleteScraperJournalCommand(ParserName, ParserType);
    }
}
