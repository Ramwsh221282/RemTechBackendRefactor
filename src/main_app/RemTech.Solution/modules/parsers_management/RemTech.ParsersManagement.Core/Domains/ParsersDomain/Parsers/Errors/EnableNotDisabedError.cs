using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class EnableNotDisabedError : IError
{
    private readonly Error _error = Error.Conflict("Попытка включить не отключенный парсер.");

    public Error Read() => _error;

    public static implicit operator Status(EnableNotDisabedError error) => new(error._error);
}
