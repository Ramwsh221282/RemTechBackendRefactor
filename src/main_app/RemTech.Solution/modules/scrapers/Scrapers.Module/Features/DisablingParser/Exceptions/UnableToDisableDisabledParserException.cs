namespace Scrapers.Module.Features.DisablingParser.Models;

internal sealed class UnableToDisableDisabledParserException : Exception
{
    public UnableToDisableDisabledParserException()
        : base("Нельзя выключить отключенный парсер.") { }

    public UnableToDisableDisabledParserException(Exception inner)
        : base("Нельзя выключить отключенный парсер.", inner) { }
}
