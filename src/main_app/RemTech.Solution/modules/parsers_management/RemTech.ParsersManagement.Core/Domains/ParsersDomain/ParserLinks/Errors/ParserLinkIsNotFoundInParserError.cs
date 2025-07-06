using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class ParserLinkIsNotFoundInParserError : IError
{
    private readonly Error _error;

    public ParserLinkIsNotFoundInParserError(IParser parser)
    {
        Guid id = parser.Identification().ReadId();
        string name = parser.Identification().ReadName();
        string text = $"Парсер с ID: {id} и с названием: {name} не содержит ссылку.";
        _error = (text, ErrorCodes.NotFound);
    }

    public Error Read() => _error;

    public static implicit operator Status<IParserLink>(ParserLinkIsNotFoundInParserError error) =>
        error._error;

    public static implicit operator Status<ParserStatisticsIncreasement>(
        ParserLinkIsNotFoundInParserError error
    ) => error._error;
}
