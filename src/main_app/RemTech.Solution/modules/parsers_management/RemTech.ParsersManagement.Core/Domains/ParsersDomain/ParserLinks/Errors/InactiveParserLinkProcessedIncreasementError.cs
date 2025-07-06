using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;

public sealed class InactiveParserLinkProcessedIncreasementError : IError
{
    private readonly Error _error;

    public InactiveParserLinkProcessedIncreasementError(IParserLink link)
    {
        Guid id = link.Identification().ReadId();
        string name = link.Identification().ReadName();
        string text =
            $"Нельзя увеличить количество обработанных данных у неактивной ссылки: ID: {id}, Название: {name}.";
        _error = (text, ErrorCodes.Conflict);
    }

    public Error Read() => _error;

    public static implicit operator Status<ParserStatisticsIncreasement>(
        InactiveParserLinkProcessedIncreasementError error
    ) => error._error;
}
