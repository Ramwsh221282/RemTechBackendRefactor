namespace Scrapers.Module.Domain.JournalsContext;

internal sealed class ScraperJournalOutputSource<TSourceType>(
    IScraperJournalOutputSource<TSourceType> source
) : IScraperJournalOutputSource<TSourceType>
    where TSourceType : IScraperJournalOutputSource<TSourceType>
{
    public void Behave() => source.Behave();

    public Task BehaveAsync() => source.BehaveAsync();

    public TSourceType Accept(
        Guid id,
        string scraperName,
        string scraperType,
        DateTime createdAt,
        DateTime? completedAt
    ) => source.Accept(id, scraperName, scraperType, createdAt, completedAt);
}
