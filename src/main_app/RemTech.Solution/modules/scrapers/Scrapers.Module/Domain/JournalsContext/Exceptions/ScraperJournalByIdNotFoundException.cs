namespace Scrapers.Module.Domain.JournalsContext.Exceptions;

internal sealed class ScraperJournalByIdNotFoundException() : Exception("Журнал не найден.")
{
    private const string Message = "Журнал не найден";

    public void Log(Serilog.ILogger logger) => logger.Error("{Message}.", Message);
}
