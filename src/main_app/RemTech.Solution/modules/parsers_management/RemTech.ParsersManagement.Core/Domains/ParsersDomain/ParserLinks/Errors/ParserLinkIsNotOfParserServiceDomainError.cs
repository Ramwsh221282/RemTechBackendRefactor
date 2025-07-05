using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class ParserLinkIsNotOfParserServiceDomainError : IError
{
    private readonly Error _error;

    public ParserLinkIsNotOfParserServiceDomainError(IParserLink link, IParser parser)
    {
        string linkUrl = link.ReadUrl().Read();
        string parserDomain = parser.Domain();
        string errorText = $"URL ссылки: {linkUrl} не содержит домен парсера: {parserDomain}";
        _error = Error.Conflict(errorText);
    }

    public Error Read() => _error;

    public static implicit operator Status<IParserLink>(
        ParserLinkIsNotOfParserServiceDomainError error
    ) => error._error;
}
