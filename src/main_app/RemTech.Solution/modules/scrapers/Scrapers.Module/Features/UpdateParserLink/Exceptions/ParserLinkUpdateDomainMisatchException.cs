namespace Scrapers.Module.Features.UpdateParserLink.Exceptions;

internal sealed class ParserLinkUpdateDomainMisatchException : Exception
{
    public ParserLinkUpdateDomainMisatchException(string domain)
        : base($"Новая ссылка не соответствует домену {domain}") { }

    public ParserLinkUpdateDomainMisatchException(string domain, Exception inner)
        : base($"Новая ссылка не соответствует домену {domain}", inner) { }
}
