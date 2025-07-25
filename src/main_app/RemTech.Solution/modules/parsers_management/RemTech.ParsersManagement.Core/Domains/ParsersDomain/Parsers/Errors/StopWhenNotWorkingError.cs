using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class StopWhenNotWorkingError : IError
{
    private readonly Error _error;

    public StopWhenNotWorkingError()
    {
        string text = $"Нельзя закончить работу, если парсер не в рабочем состоянии.";
        _error = Error.Conflict(text);
    }

    public Error Read() => _error;

    public static implicit operator Status(StopWhenNotWorkingError error) => new(error._error);
}
