namespace Scrapers.Module.Domain.JournalsContext.Persistance;

internal interface IScraperJournals
{
    Task<ScraperJournal> ById(Guid id, CancellationToken ct = default);
    Task RemoveById(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ScraperJournal>> GetPaged(
        string name,
        string type,
        int page,
        DateTime? from,
        DateTime? to,
        CancellationToken ct = default
    );

    Task<long> GetCount(string name, string type, CancellationToken ct = default);
}
