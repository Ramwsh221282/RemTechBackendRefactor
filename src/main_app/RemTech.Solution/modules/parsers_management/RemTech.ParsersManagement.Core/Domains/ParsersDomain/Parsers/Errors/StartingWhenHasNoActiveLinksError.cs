using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class StartingWhenHasNoActiveLinksError : IError
{
    private readonly Error _error;

    public StartingWhenHasNoActiveLinksError(IParser parser)
    {
        Guid id = parser.Identification().ReadId();
        string name = parser.Identification().ReadName();
        string type = parser.Identification().ReadType().Read();
        string domain = parser.Domain().Read();
        string text =
            $"Парсер с ID: {id}, названием: {name}, типом: {type}, доменом: {domain} не содержит активные ссылки. Нельзя начать парсинг.";
        _error = (text, ErrorCodes.Conflict);
    }

    public Error Read() => _error;

    public static implicit operator Status(StartingWhenHasNoActiveLinksError error) =>
        new(error._error);
}
