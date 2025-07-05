using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class ParserLinkIsNotOfParserError : IError
{
    private readonly Error _error;

    public ParserLinkIsNotOfParserError(IParserLink link, IParser parser)
    {
        string text =
            $"Парсер с ID: {parser.Identification().ReadId().GuidValue()} и названием: {parser.Identification().ReadName().NameString()} не обладатель ссылки с ID: {link.Identification().ReadId().GuidValue()} и названием: {link.Identification().ReadName().NameString()}";
        _error = Error.Conflict(text);
    }

    public Error Read() => _error;

    public static implicit operator Error(ParserLinkIsNotOfParserError error) => error._error;

    public static implicit operator Status(ParserLinkIsNotOfParserError error) => error._error;

    public static implicit operator Status<IParserLink>(ParserLinkIsNotOfParserError error) =>
        error._error;

    public static implicit operator Status<ParserStatisticsIncreasement>(
        ParserLinkIsNotOfParserError error
    ) => error._error;
}
