using Scrapers.Module.Features.CreateNewParser.Exceptions;

namespace Scrapers.Module.Features.CreateNewParser.Models;

internal sealed record NewParserDomain
{
    private const int AllowedNameLength = 20;
    public string Domain { get; }

    private NewParserDomain(string domain) => Domain = domain;

    public static NewParserDomain Create(string domain)
    {
        if (string.IsNullOrWhiteSpace(domain))
            throw new ParserDomainEmptyException();
        if (domain.Length > AllowedNameLength)
            throw new ParserDomainExceesLengthException(domain, AllowedNameLength);
        return new NewParserDomain(domain);
    }
}
