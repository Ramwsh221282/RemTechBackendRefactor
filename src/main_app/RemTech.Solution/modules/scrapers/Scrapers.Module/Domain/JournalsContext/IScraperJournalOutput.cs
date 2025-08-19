namespace Scrapers.Module.Domain.JournalsContext;

internal interface IScraperJournalOutput
{
    TScraperJournalOutputSource PrintTo<TScraperJournalOutputSource>(
        TScraperJournalOutputSource source
    )
        where TScraperJournalOutputSource : IScraperJournalOutputSource<TScraperJournalOutputSource>;
}
