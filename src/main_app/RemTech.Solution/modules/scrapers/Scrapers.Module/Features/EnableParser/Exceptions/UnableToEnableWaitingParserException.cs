namespace Scrapers.Module.Features.EnableParser.Exceptions;

internal sealed class UnableToEnableWaitingParserException : Exception
{
    public UnableToEnableWaitingParserException()
        : base($"Нельзя включить ожидающий парсер.") { }

    public UnableToEnableWaitingParserException(Exception inner)
        : base($"Нельзя включить ожидающий парсер.", inner) { }
}
