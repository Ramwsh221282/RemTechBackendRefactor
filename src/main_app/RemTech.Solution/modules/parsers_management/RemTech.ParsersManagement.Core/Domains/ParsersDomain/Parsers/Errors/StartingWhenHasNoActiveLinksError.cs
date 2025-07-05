using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class StartingWhenHasNoActiveLinksError : IError
{
    private readonly Error _error;

    public StartingWhenHasNoActiveLinksError(IParser parser)
    {
        string text =
            $"Парсер с ID: {parser.Identification().ReadId().GuidValue()}, названием: {parser.Identification().ReadName().NameString()}, типом: {parser.Identification().ReadType().Read()}, доменом: {parser.Domain().Read().NameString()} не содержит активные ссылки. Нельзя начать парсинг.";
        _error = Error.Conflict(text);
    }

    public Error Read() => _error;

    public static implicit operator Status(StartingWhenHasNoActiveLinksError error) => error._error;
}