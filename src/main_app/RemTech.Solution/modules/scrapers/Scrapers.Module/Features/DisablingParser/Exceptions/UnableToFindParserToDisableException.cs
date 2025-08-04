namespace Scrapers.Module.Features.DisablingParser.Exceptions;

internal sealed class UnableToFindParserToDisableException : Exception
{
    public UnableToFindParserToDisableException()
        : base("Не удается найти парсер для отключения.") { }

    public UnableToFindParserToDisableException(Exception inner)
        : base("Не удается найти парсер для отключения.", inner) { }
}
