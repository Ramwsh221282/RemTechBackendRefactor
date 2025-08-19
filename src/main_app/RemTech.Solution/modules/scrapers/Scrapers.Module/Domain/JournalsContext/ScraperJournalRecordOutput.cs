namespace Scrapers.Module.Domain.JournalsContext;

internal sealed class ScraperJournalRecordOutput(
    Guid id,
    Guid journalId,
    string action,
    string text,
    DateTime createdAt
)
{
    public TType PrintTo<TType>(TType source)
        where TType : IScraperJournalRecordOutputSource<TType> =>
        source.Accept(id, journalId, action, text, createdAt);
}
