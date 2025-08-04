namespace Scrapers.Module.Features.UpdateWaitDays.Exceptions;

internal sealed class InvalidParserWaitDaysToUpdateException : Exception
{
    public InvalidParserWaitDaysToUpdateException(int waitDays)
        : base($"Время ожидания {waitDays} дней некорректно.") { }

    public InvalidParserWaitDaysToUpdateException(int waitDays, Exception inner)
        : base($"Время ожидания {waitDays} дней некорректно.", inner) { }
}
