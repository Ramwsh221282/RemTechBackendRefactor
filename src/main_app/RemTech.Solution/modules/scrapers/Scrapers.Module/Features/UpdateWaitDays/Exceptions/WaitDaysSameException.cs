namespace Scrapers.Module.Features.UpdateWaitDays.Exceptions;

internal sealed class WaitDaysSameException : Exception
{
    public WaitDaysSameException(string parserName, string parserType, int waitDays)
        : base($"Дни ожидания парсера {parserName} {parserType} уже {waitDays}") { }

    public WaitDaysSameException(
        string parserName,
        string parserType,
        int waitDays,
        Exception inner
    )
        : base($"Дни ожидания парсера {parserName} {parserType} уже {waitDays}", inner) { }
}
