namespace Scrapers.Module.Features.CreateNewParser.Exceptions;

internal sealed class ParserDomainEmptyException : Exception
{
    public ParserDomainEmptyException()
        : base("Название домена парсера было пустым") { }

    public ParserDomainEmptyException(Exception inner)
        : base("Название домена парсера было пустым.", inner) { }
}
