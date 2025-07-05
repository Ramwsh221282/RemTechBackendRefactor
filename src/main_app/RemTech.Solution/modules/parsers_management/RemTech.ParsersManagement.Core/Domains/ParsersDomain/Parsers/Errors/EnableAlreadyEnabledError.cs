using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class EnableAlreadyEnabledError : IError
{
    private readonly Error _error = Error.Conflict("Нельзя включить уже включенный парсер.");

    public Error Read() => _error;

    public static implicit operator Status(EnableAlreadyEnabledError error) => error._error;
}
