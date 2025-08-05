namespace Scrapers.Module.Features.FinishParserLink.Exceptions;

internal sealed class ParserLinkToFinishNotFoundException : Exception
{
    public ParserLinkToFinishNotFoundException(
        string parserName,
        string linkName,
        string parserType
    )
        : base($"Не удается найти ссылку {parserName} {linkName} {parserType}") { }

    public ParserLinkToFinishNotFoundException(
        string parserName,
        string linkName,
        string parserType,
        Exception inner
    )
        : base($"Не удается найти ссылку {parserName} {linkName} {parserType}", inner) { }
}
