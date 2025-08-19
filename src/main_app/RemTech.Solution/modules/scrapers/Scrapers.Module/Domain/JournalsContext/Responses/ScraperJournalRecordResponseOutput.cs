namespace Scrapers.Module.Domain.JournalsContext.Responses;

internal sealed class ScraperJournalRecordResponseOutput
    : IScraperJournalRecordOutputSource<ScraperJournalRecordResponseOutput>
{
    private readonly Guid _id;
    private readonly Guid _journalId;
    private readonly string _action;
    private readonly string _text;
    private readonly DateTime _createdAt;
    private ScraperJournalRecordResponse _response = ScraperJournalRecordResponse.Default();

    public ScraperJournalRecordResponse Read() => _response;

    public ScraperJournalRecordResponseOutput Accept(
        Guid id,
        Guid journalId,
        string action,
        string text,
        DateTime createdAt
    ) => new(id, journalId, action, text, createdAt);

    public void Behave()
    {
        _response = new ScraperJournalRecordResponse(_id, _journalId, _action, _text, _createdAt);
    }

    public Task BehaveAsync()
    {
        Behave();
        return Task.CompletedTask;
    }

    public ScraperJournalRecordResponseOutput()
        : this(Guid.Empty, Guid.Empty, string.Empty, string.Empty, DateTime.MinValue) { }

    private ScraperJournalRecordResponseOutput(
        Guid id,
        Guid journalId,
        string action,
        string text,
        DateTime createdAt
    )
    {
        _id = id;
        _journalId = journalId;
        _action = action;
        _createdAt = createdAt;
        _text = text;
    }
}
