namespace Scrapers.Module.Domain.JournalsContext;

internal interface IScraperJournalOutputSource<TType>
    where TType : IScraperJournalOutputSource<TType>
{
    void Behave();
    Task BehaveAsync();
    TType Accept(
        Guid id,
        string scraperName,
        string scraperType,
        DateTime createdAt,
        DateTime? completedAt
    );
}
