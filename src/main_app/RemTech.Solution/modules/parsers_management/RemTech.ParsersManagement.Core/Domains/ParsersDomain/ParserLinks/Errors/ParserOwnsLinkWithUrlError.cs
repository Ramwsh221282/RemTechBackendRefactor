using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class ParserOwnsLinkWithUrlError : IError
{
    private readonly Error _error;

    public ParserOwnsLinkWithUrlError(IParser parser, IParserLink link)
    {
        Guid parserId = parser.Identification().ReadId();
        string parserName = parser.Identification().ReadName();
        string linkUrl = link.ReadUrl();
        string message =
            $"Парсер с ID: {parserId} и названием: {parserName} уже содержит ссылку с URL: {linkUrl}";
        _error = (message, ErrorCodes.Conflict);
    }

    public Error Read() => _error;

    public static implicit operator Status<IParserLink>(ParserOwnsLinkWithUrlError error) =>
        error._error;
}
