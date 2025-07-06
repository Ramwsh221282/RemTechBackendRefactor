using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class ParserLinkWithIdNotFoundError : IError
{
    private readonly Error _error;

    public ParserLinkWithIdNotFoundError(IParser parser, NotEmptyGuid linkId)
        : this(parser, (Guid)linkId) { }

    public ParserLinkWithIdNotFoundError(IParser parser, Guid linkId)
    {
        Guid parserId = parser.Identification().ReadId();
        string name = parser.Identification().ReadName();
        string text =
            $"Парсер с ID: {parserId} и названием {name} не содержит ссылку с ID: {linkId}";
        _error = (text, ErrorCodes.NotFound);
    }

    public Error Read() => _error;

    public static implicit operator Status<IParserLink>(ParserLinkWithIdNotFoundError error) =>
        error._error;
}
