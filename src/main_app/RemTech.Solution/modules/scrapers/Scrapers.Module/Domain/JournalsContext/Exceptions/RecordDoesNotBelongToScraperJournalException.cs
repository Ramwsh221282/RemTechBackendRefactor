namespace Scrapers.Module.Domain.JournalsContext.Exceptions;

internal sealed class RecordDoesNotBelongToScraperJournalException(Guid recordId, Guid journalId)
    : Exception($"Запись журнала {recordId} не соответствует журналу {journalId}")
{
    public void Log(Serilog.ILogger logger)
    {
        string message = $"Запись журнала {recordId} не соответствует журналу {journalId}";
        logger.Error("{Message}.", message);
    }
}
