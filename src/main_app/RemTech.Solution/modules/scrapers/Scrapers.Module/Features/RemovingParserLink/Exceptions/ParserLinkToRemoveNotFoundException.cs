namespace Scrapers.Module.Features.RemovingParserLink.Exceptions;

internal sealed class ParserLinkToRemoveNotFoundException : Exception
{
    public ParserLinkToRemoveNotFoundException(
        string linkName,
        string parserName,
        string parserType
    )
        : base($"Не удается найти ссылку {linkName} {parserName} {parserType}") { }

    public ParserLinkToRemoveNotFoundException(
        string linkName,
        string parserName,
        string parserType,
        Exception inner
    )
        : base($"Не удается найти ссылку {linkName} {parserName} {parserType}", inner) { }
}
