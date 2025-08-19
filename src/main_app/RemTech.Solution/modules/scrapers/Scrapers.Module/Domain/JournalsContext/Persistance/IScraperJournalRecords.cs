namespace Scrapers.Module.Domain.JournalsContext.Persistance;

internal interface IScraperJournalRecords
{
    Task<IEnumerable<ScraperJournalRecord>> GetPaged(
        Guid journalId,
        int page,
        string? text = null,
        CancellationToken ct = default
    );

    Task<long> GetCount(Guid journalId, CancellationToken ct = default);
}
