using Scrapers.Module.Domain.JournalsContext.Exceptions;

namespace Scrapers.Module.Domain.JournalsContext;

internal sealed class ScraperJournal
{
    private readonly Guid _id;
    private readonly string _scraperName;
    private readonly string _scraperType;
    private readonly DateTime _createdAt;
    private readonly DateTime? _completedAt;
    private readonly List<ScraperJournalRecord> _records;

    private ScraperJournal(
        Guid id,
        string scraperName,
        string scraperType,
        DateTime createdAt,
        DateTime? completedAt,
        IEnumerable<ScraperJournalRecord> records
    )
    {
        _id = id;
        _scraperName = scraperName;
        _scraperType = scraperType;
        _createdAt = createdAt;
        _completedAt = completedAt;
        _records = new List<ScraperJournalRecord>(records);
    }

    public bool HasCompleted() => _completedAt.HasValue;

    public ScraperJournal Complete(DateTime completedAt)
    {
        return HasCompleted()
            ? throw new ScraperJournalAlreadyCompletedException(
                _scraperName,
                _scraperType,
                _createdAt
            )
            : Create(_id, _scraperName, _scraperType, _createdAt, completedAt, _records);
    }

    public ScraperJournalOutput ProduceOutput() =>
        new(_id, _scraperName, _scraperType, _createdAt, _completedAt);

    public ScraperJournalRecord WithRecord(
        Guid id,
        string action,
        string text,
        DateTime createdAt
    ) => WithRecord(_id, id, action, text, createdAt);

    public ScraperJournalRecord WithRecord(
        Guid journalId,
        Guid id,
        string action,
        string text,
        DateTime createdAt
    )
    {
        if (_id != journalId)
            throw new RecordDoesNotBelongToScraperJournalException(id, _id);
        ScraperJournalRecord record = new ScraperJournalRecord(
            id,
            journalId,
            action,
            text,
            createdAt
        );
        _records.Add(record);
        return record;
    }

    public static ScraperJournal Create(string scraperName, string scraperType)
    {
        Guid id = Guid.NewGuid();
        DateTime createdAt = DateTime.UtcNow;
        return Create(id, scraperName, scraperType, createdAt, null, []);
    }

    public static ScraperJournal Create(
        Guid id,
        string scraperName,
        string scraperType,
        DateTime createdAt,
        DateTime? completedAt,
        IEnumerable<ScraperJournalRecord> records
    ) => new(id, scraperName, scraperType, createdAt, completedAt, records);

    public TScraperJournalOutputSource PrintTo<TScraperJournalOutputSource>(
        TScraperJournalOutputSource source
    )
        where TScraperJournalOutputSource : IScraperJournalOutputSource<TScraperJournalOutputSource> =>
        ProduceOutput().PrintTo(source);
}
