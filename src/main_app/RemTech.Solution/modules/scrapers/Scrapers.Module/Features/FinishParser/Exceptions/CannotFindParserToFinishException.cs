namespace Scrapers.Module.Features.FinishParser.Exceptions;

internal sealed class CannotFindParserToFinishException : Exception
{
    public CannotFindParserToFinishException(string parserName, string parserType)
        : base($"Не удается найти парсер {parserName} {parserType}.") { }

    public CannotFindParserToFinishException(string parserName, string parserType, Exception inner)
        : base($"Не удается найти парсер {parserName} {parserType}.", inner) { }
}
