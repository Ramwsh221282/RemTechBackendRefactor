using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class ParserLinkIsNotOfParserError : IError
{
    private readonly Error _error;

    public ParserLinkIsNotOfParserError(IParserLink link, IParser parser)
    {
        Guid parserId = parser.Identification().ReadId();
        Guid linkId = link.Identification().ReadId();
        string parserName = parser.Identification().ReadName();
        string linkName = link.Identification().ReadName();
        string text =
            $"Парсер с ID: {parserId}, названием: {parserName} не обладатель ссылки с ID: {linkId} и названием: {linkName}. ";
        _error = (text, ErrorCodes.Conflict);
    }

    public Error Read() => _error;

    public static implicit operator Error(ParserLinkIsNotOfParserError error) => error._error;

    public static implicit operator Status(ParserLinkIsNotOfParserError error) => new(error._error);

    public static implicit operator Status<IParserLink>(ParserLinkIsNotOfParserError error) =>
        error._error;

    public static implicit operator Status<ParserStatisticsIncreasement>(
        ParserLinkIsNotOfParserError error
    ) => error._error;
}
