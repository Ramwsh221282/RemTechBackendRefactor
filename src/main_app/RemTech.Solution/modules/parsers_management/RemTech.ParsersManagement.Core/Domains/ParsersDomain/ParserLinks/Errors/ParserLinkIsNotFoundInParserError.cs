using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class ParserLinkIsNotFoundInParserError : IError
{
    private readonly Error _error;

    public ParserLinkIsNotFoundInParserError(IParser parser)
    {
        string text =
            $"Парсер с ID: {parser.Identification().ReadId().GuidValue()} и с названием: {parser.Identification().ReadName().NameString().StringValue()} не содержит ссылку.";
        _error = Error.NotFound(text);
    }

    public Error Read() => _error;

    public static implicit operator Status<IParserLink>(ParserLinkIsNotFoundInParserError error) =>
        error._error;

    public static implicit operator Status<ParserStatisticsIncreasement>(
        ParserLinkIsNotFoundInParserError error
    ) => error._error;
}
