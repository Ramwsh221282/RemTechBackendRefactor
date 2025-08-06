namespace Scrapers.Module.Features.ChangeParserState.Exception;

internal sealed class ParserStateToChangeNotFoundException : System.Exception
{
    public ParserStateToChangeNotFoundException(string parserName, string parserType)
        : base($"Парсер {parserName} {parserType} не найден") { }

    public ParserStateToChangeNotFoundException(
        string parserName,
        string parserType,
        System.Exception inner
    )
        : base($"Парсер {parserName} {parserType} не найден", inner) { }
}
