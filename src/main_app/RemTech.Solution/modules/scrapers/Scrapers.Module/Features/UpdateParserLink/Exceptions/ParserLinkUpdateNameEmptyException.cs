namespace Scrapers.Module.Features.UpdateParserLink.Exceptions;

internal sealed class ParserLinkUpdateNameEmptyException : Exception
{
    public ParserLinkUpdateNameEmptyException()
        : base("Новое значение названия ссылки было пустым.") { }

    public ParserLinkUpdateNameEmptyException(Exception ex)
        : base("Новое значение названия ссылки было пустым.", ex) { }
}
