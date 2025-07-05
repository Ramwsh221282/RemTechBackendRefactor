using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class IncreaseParserProcessedWhenNotWorking : IError
{
    private readonly Error _error;

    public IncreaseParserProcessedWhenNotWorking()
    {
        string text = string.Intern(
            "Нельзя увеличить количество обработанных объявлений парсеру, когда он не работает."
        );
        _error = Error.Conflict(text);
    }

    public Error Read() => _error;

    public static implicit operator Status<ParserStatisticsIncreasement>(
        IncreaseParserProcessedWhenNotWorking error
    ) => error._error;
}
