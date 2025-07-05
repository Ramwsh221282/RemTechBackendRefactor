using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class InactiveParserLinkProcessedIncreasementError : IError
{
    private readonly Error _error;

    public InactiveParserLinkProcessedIncreasementError(IParserLink link)
    {
        string text =
            $"Нельзя увеличить количество обработанных данных у неактивной ссылки: {link.Identification().ReadId().GuidValue()}, {link.Identification().ReadName().NameString().StringValue()}";
        _error = Error.Conflict(text);
    }

    public Error Read() => _error;

    public static implicit operator Status<ParserStatisticsIncreasement>(
        InactiveParserLinkProcessedIncreasementError error
    ) => error._error;
}
