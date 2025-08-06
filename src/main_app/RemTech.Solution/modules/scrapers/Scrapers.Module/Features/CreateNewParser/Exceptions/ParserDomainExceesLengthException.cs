namespace Scrapers.Module.Features.CreateNewParser.Exceptions;

internal sealed class ParserDomainExceesLengthException : Exception
{
    public ParserDomainExceesLengthException(string domain, int length)
        : base($"Домен парсера {domain} превышает длину {length} символов.") { }

    public ParserDomainExceesLengthException(string domain, int length, Exception inner)
        : base($"Домен парсера {domain} превышает длину {length} символов.", inner) { }
}
