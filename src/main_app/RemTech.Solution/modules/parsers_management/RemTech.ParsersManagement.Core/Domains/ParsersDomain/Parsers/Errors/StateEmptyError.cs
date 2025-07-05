using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class StateEmptyError : IError
{
    private readonly Error _error = Error.Conflict("Строка состояния парсера была пустой.");

    public Error Read() => _error;

    public static implicit operator Status(StateEmptyError error) => error._error;
}
