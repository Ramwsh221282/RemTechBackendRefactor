namespace Scrapers.Module.Features.InstantlyDisableParser.Exceptions;

internal sealed class UnableToFindParserToInstantlyDisableException : Exception
{
    public UnableToFindParserToInstantlyDisableException(string parserName, string parserType)
        : base($"Парсер {parserType} {parserType} не найден.") { }

    public UnableToFindParserToInstantlyDisableException(
        string parserName,
        string parserType,
        Exception inner
    )
        : base($"Парсер {parserType} {parserType} не найден.", inner) { }
}
