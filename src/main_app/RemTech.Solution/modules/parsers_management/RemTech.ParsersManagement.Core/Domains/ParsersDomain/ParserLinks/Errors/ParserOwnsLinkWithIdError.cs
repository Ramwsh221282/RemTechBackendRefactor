using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class ParserOwnsLinkWithIdError : IError
{
    private readonly Error _error;

    public ParserOwnsLinkWithIdError(IParser parser, IParserLink link)
    {
        Guid parserId = parser.Identification().ReadId();
        string parserName = parser.Identification().ReadName();
        Guid linkId = link.Identification().ReadId();
        string message =
            $"Парсер с ID: {parserId} и названием: {parserName} уже содержит ссылку с ID: {linkId}";
        _error = (message, ErrorCodes.Conflict);
    }

    public Error Read() => _error;

    public static implicit operator Status<IParserLink>(ParserOwnsLinkWithIdError error) =>
        error._error;
}
