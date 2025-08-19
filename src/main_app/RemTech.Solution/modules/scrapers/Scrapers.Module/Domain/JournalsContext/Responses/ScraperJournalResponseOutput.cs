namespace Scrapers.Module.Domain.JournalsContext.Responses;

internal sealed class ScraperJournalResponseOutput
    : IScraperJournalOutputSource<ScraperJournalResponseOutput>
{
    private ScraperJournalResponse _response = ScraperJournalResponse.Default();
    private readonly Guid _id;
    private readonly string _name;
    private readonly string _type;
    private readonly DateTime _createdAt;
    private readonly DateTime? _completedAt;

    public void Behave()
    {
        _response = new ScraperJournalResponse(_id, _name, _type, _createdAt, _completedAt);
    }

    public ScraperJournalResponse Read() => _response;

    public Task BehaveAsync()
    {
        Behave();
        return Task.CompletedTask;
    }

    public ScraperJournalResponseOutput Accept(
        Guid id,
        string scraperName,
        string scraperType,
        DateTime createdAt,
        DateTime? completedAt
    ) => new(id, scraperName, scraperType, createdAt, completedAt);

    public ScraperJournalResponseOutput()
        : this(Guid.Empty, string.Empty, string.Empty, DateTime.MinValue, DateTime.MinValue) { }

    private ScraperJournalResponseOutput(
        Guid id,
        string name,
        string type,
        DateTime createdAt,
        DateTime? completedAt
    )
    {
        _id = id;
        _name = name;
        _type = type;
        _createdAt = createdAt;
        _completedAt = completedAt;
    }
}
