using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class ParserOwnsLinkWithNameError : IError
{
    private readonly Error _error;

    public ParserOwnsLinkWithNameError(IParser parser, IParserLink link)
    {
        _error = Error.Conflict(
            $"Парсер с ID: {parser.Identification().ReadId().GuidValue()} и названием: {parser.Identification().ReadName().NameString()} уже содержит ссылку с названием: {link.Identification().ReadName().NameString()}"
        );
    }

    public Error Read() => _error;

    public static implicit operator Status(ParserOwnsLinkWithNameError error) => error._error;

    public static implicit operator Status<IParserLink>(ParserOwnsLinkWithNameError error) =>
        error._error;
}
