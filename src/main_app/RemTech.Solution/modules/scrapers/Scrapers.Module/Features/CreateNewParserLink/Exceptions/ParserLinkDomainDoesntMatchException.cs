namespace Scrapers.Module.Features.CreateNewParserLink.Exceptions;

internal sealed class ParserLinkDomainDoesntMatchException : Exception
{
    public ParserLinkDomainDoesntMatchException(string url, string domain)
        : base($"URL ссылки {url} не соответствует домену парсера {domain}") { }

    public ParserLinkDomainDoesntMatchException(string url, string domain, Exception inner)
        : base($"URL ссылки {url} не соответствует домену парсера {domain}", inner) { }
}
