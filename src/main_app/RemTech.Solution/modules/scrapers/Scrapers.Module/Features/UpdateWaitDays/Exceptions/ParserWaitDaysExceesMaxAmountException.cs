namespace Scrapers.Module.Features.UpdateWaitDays.Exceptions;

internal sealed class ParserWaitDaysExceesMaxAmountException : Exception
{
    public ParserWaitDaysExceesMaxAmountException(int waitdays, int limit)
        : base($"Время ожидания {waitdays} превышает допустимое {limit}") { }

    public ParserWaitDaysExceesMaxAmountException(int waitdays, int limit, Exception inner)
        : base($"Время ожидания {waitdays} превышает допустимое {limit}", inner) { }
}
