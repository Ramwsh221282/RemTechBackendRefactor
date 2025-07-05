using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class ParserOwnsLinkWithUrlError : IError
{
    private readonly Error _error;

    public ParserOwnsLinkWithUrlError(IParser parser, IParserLink link)
    {
        _error = Error.Conflict(
            $"Парсер с ID: {parser.Identification().ReadId().GuidValue()} и названием: {parser.Identification().ReadName().NameString()} уже содержит ссылку с URL: {link.ReadUrl().Read().StringValue()}"
        );
    }

    public Error Read() => _error;

    public static implicit operator Status<IParserLink>(ParserOwnsLinkWithUrlError error) =>
        error._error;
}
