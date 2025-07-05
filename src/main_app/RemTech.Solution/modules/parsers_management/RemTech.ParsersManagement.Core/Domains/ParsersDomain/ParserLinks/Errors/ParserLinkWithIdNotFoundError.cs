using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class ParserLinkWithIdNotFoundError : IError
{
    private readonly Error _error;

    public ParserLinkWithIdNotFoundError(IParser parser, NotEmptyGuid linkId)
        : this(parser, linkId.GuidValue()) { }

    public ParserLinkWithIdNotFoundError(IParser parser, Guid linkId)
    {
        string text =
            $"Парсер с ID: {parser.Identification().ReadId().GuidValue()} и названием {parser.Identification().ReadName().NameString().StringValue()} не содержит ссылку с ID: {linkId}";
        _error = Error.NotFound(text);
    }

    public Error Read() => _error;

    public static implicit operator Status<IParserLink>(ParserLinkWithIdNotFoundError error) =>
        error._error;
}
