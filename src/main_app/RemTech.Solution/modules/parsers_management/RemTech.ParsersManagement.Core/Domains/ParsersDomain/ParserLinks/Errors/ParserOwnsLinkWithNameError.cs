using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class ParserOwnsLinkWithNameError : IError
{
    private readonly Error _error;

    public ParserOwnsLinkWithNameError(IParser parser, IParserLink link)
    {
        Guid parserId = parser.Identification().ReadId();
        string parserName = parser.Identification().ReadName();
        string linkName = link.Identification().ReadName();
        string message =
            $"Парсер с ID: {parserId} и названием: {parserName} уже содержит ссылку с названием: {linkName}";
        _error = (message, ErrorCodes.Conflict);
    }

    public Error Read() => _error;

    public static implicit operator Status(ParserOwnsLinkWithNameError error) => new(error._error);

    public static implicit operator Status<IParserLink>(ParserOwnsLinkWithNameError error) =>
        error._error;
}
