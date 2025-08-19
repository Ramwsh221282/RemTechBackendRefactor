namespace Scrapers.Module.Domain.JournalsContext;

internal sealed class ScraperJournalOutput(
    Guid id,
    string scraperName,
    string scraperType,
    DateTime createdAt,
    DateTime? completedAt
) : IScraperJournalOutput
{
    public TScraperJournalOutputSource PrintTo<TScraperJournalOutputSource>(
        TScraperJournalOutputSource source
    )
        where TScraperJournalOutputSource : IScraperJournalOutputSource<TScraperJournalOutputSource> =>
        source.Accept(id, scraperName, scraperType, createdAt, completedAt);
}
