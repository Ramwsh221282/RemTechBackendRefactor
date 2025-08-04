namespace Scrapers.Module.Features.DisablingParser.Models;

internal sealed class UnableToDisableWorkingParserException : Exception
{
    public UnableToDisableWorkingParserException()
        : base("Нельзя выключить работающий парсер.") { }

    public UnableToDisableWorkingParserException(Exception inner)
        : base("Нельзя выключить работающий парсер.") { }
}
