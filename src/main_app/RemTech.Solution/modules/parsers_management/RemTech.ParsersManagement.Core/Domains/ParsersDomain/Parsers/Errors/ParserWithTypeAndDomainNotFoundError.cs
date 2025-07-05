using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class ParserWithTypeAndDomainNotFoundError : IError
{
    private readonly Error _error;

    public ParserWithTypeAndDomainNotFoundError(ParsingType type, NotEmptyString domain)
        : this(type.Read(), domain) { }

    public ParserWithTypeAndDomainNotFoundError(NotEmptyString type, NotEmptyString domain)
        : this(type.StringValue(), domain.StringValue()) { }

    public ParserWithTypeAndDomainNotFoundError(string type, string parserDomain)
        : this($"Парсер с типом: {type} и доменом: {parserDomain} не найден.") { }

    public ParserWithTypeAndDomainNotFoundError(string errorText) =>
        _error = Error.NotFound(errorText);

    public Error Read() => _error;

    public static implicit operator Status<IParser>(ParserWithTypeAndDomainNotFoundError error) =>
        error._error;
}
