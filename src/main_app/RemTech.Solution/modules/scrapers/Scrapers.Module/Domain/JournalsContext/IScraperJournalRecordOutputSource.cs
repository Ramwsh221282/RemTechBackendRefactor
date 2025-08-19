namespace Scrapers.Module.Domain.JournalsContext;

internal interface IScraperJournalRecordOutputSource<TType>
    where TType : IScraperJournalRecordOutputSource<TType>
{
    TType Accept(Guid id, Guid journalId, string action, string text, DateTime createdAt);
    void Behave();
    Task BehaveAsync();
}
