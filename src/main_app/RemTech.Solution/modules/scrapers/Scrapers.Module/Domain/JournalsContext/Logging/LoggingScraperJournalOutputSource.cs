using System.Globalization;

namespace Scrapers.Module.Domain.JournalsContext.Logging;

internal sealed class LoggingScraperJournalOutputSource
    : IScraperJournalOutputSource<LoggingScraperJournalOutputSource>
{
    private readonly Serilog.ILogger _logger;
    private readonly Guid _id;
    private readonly string _scraperName;
    private readonly string _scraperType;
    private readonly DateTime _createdAt;
    private readonly DateTime? _completedAt;

    public void Behave()
    {
        _logger.Information(
            """
            SCRAPER JOURNAL INFO:
            ID: {id},
            Scraper: {scraperName} {scraperType}
            Created At: {createdAt}
            Finished At: {completedAt}
            """,
            _id,
            _scraperName,
            _scraperType,
            _createdAt.ToString(CultureInfo.InvariantCulture),
            _completedAt.HasValue
                ? _completedAt.Value.ToString(CultureInfo.InvariantCulture)
                : "Not completed yet"
        );
    }

    public Task BehaveAsync()
    {
        Behave();
        return Task.CompletedTask;
    }

    public LoggingScraperJournalOutputSource Accept(
        Guid id,
        string scraperName,
        string scraperType,
        DateTime createdAt,
        DateTime? completedAt
    ) => new(_logger, id, scraperName, scraperType, createdAt, completedAt);

    public LoggingScraperJournalOutputSource(Serilog.ILogger logger)
        : this(logger, Guid.Empty, string.Empty, string.Empty, DateTime.MinValue, null) { }

    private LoggingScraperJournalOutputSource(
        Serilog.ILogger logger,
        Guid id,
        string scraperName,
        string scraperType,
        DateTime createdAt,
        DateTime? completedAt
    )
    {
        _logger = logger;
        _id = id;
        _scraperName = scraperName;
        _scraperType = scraperType;
        _createdAt = createdAt;
        _completedAt = completedAt;
    }
}
