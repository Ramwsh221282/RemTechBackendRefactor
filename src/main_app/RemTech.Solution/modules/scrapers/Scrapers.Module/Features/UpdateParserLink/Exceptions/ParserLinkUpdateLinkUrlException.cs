namespace Scrapers.Module.Features.UpdateParserLink.Exceptions;

internal sealed class ParserLinkUpdateLinkUrlException : Exception
{
    public ParserLinkUpdateLinkUrlException()
        : base("Новое значение URL для ссылки было пустым.") { }

    public ParserLinkUpdateLinkUrlException(Exception inner)
        : base("Новое значение URL для ссылки было пустым.", inner) { }
}
