namespace Scrapers.Module.Features.UpdateParserLink.Exceptions;

internal sealed class ParserLinkToUpdateNotFoundException : Exception
{
    public ParserLinkToUpdateNotFoundException(
        string parserName,
        string parserType,
        string linkName
    )
        : base($"Ссылка {linkName} {parserName} {parserType} не найдена.") { }

    public ParserLinkToUpdateNotFoundException(
        string parserName,
        string parserType,
        string linkName,
        Exception ex
    )
        : base($"Ссылка {linkName} {parserName} {parserType} не найдена.", ex) { }
}
