using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class DisableDisabledParserError : IError
{
    private readonly Error _error = Error.Conflict("Нельзя выключить выключенный парсер.");

    public Error Read() => _error;

    public static implicit operator Status(DisableDisabledParserError error) => new(error._error);
}
