using System.Globalization;

namespace Scrapers.Module.Domain.JournalsContext.Exceptions;

internal sealed class ScraperJournalAlreadyCompletedException(
    string scraperName,
    string scraperType,
    DateTime createdAt
)
    : Exception(
        $"Журнал скрейпинга {scraperName} {scraperType} от {createdAt.ToString(CultureInfo.InvariantCulture)} уже завершен."
    )
{
    private const string ErrorFormat = "Журнал скрейпинга {0} {1} от {2} уже завершен.";

    public void Log(Serilog.ILogger logger)
    {
        string message = string.Format(
            ErrorFormat,
            scraperName,
            scraperType,
            createdAt.ToString(CultureInfo.InvariantCulture)
        );
        logger.Error("{Message}.", message);
    }
}
