using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class WaitDaysEmptyError : IError
{
    private readonly Error _error = Error.Validation("Дни ожидания парсера не указаны.");

    public Error Read() => _error;

    public static implicit operator Status(WaitDaysEmptyError error) => new(error.Read());
}
