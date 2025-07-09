using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class StartingWhenNotWaitingOrEnabledError : IError
{
    private readonly Error _error;

    public StartingWhenNotWaitingOrEnabledError()
    {
        string text = $"Нельзя начать работу если парсер не ожидает, или не включен.";
        _error = Error.Conflict(text);
    }

    public Error Read() => _error;

    public static implicit operator Status(StartingWhenNotWaitingOrEnabledError error) =>
        new(error._error);
}
