namespace Scrapers.Module.Features.CreateNewParserLink.Exceptions;

internal sealed class ParserWhereToPutLinkNameEmptyException : Exception
{
    public ParserWhereToPutLinkNameEmptyException()
        : base("Пустое название.") { }
}
