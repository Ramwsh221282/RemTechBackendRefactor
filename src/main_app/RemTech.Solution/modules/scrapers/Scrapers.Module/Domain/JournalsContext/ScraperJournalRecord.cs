namespace Scrapers.Module.Domain.JournalsContext;

internal sealed class ScraperJournalRecord(
    Guid id,
    Guid journalId,
    string action,
    string text,
    DateTime createdAt
)
{
    public TType PrintTo<TType>(TType source)
        where TType : IScraperJournalRecordOutputSource<TType> =>
        new ScraperJournalRecordOutput(id, journalId, action, text, createdAt).PrintTo(source);
}
