namespace Scrapers.Module.Features.ChangeParserState.Exception;

internal sealed class ParserAlreadyHasThisStateException : System.Exception
{
    public ParserAlreadyHasThisStateException(string parserName, string parserType, string newState)
        : base($"Парсер {parserName} {parserType} уже в состоянии {newState}") { }

    public ParserAlreadyHasThisStateException(
        string parserName,
        string parserType,
        string newState,
        System.Exception inner
    )
        : base($"Парсер {parserName} {parserType} уже в состоянии {newState}", inner) { }
}
