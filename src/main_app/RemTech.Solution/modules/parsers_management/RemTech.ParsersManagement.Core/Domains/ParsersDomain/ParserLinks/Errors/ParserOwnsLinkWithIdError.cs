using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class ParserOwnsLinkWithIdError : IError
{
    private readonly Error _error;

    public ParserOwnsLinkWithIdError(IParser parser, IParserLink link)
    {
        _error = Error.Conflict(
            $"Парсер с ID: {parser.Identification().ReadId().GuidValue()} и названием: {parser.Identification().ReadName().NameString()} уже содержит ссылку с ID: {link.Identification().ReadId().GuidValue()}"
        );
    }

    public Error Read() => _error;

    public static implicit operator Status<IParserLink>(ParserOwnsLinkWithIdError error) =>
        error._error;
}
