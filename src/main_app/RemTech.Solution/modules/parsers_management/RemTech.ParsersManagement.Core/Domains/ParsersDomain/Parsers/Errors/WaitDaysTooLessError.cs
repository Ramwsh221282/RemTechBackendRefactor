using RemTech.Core.Shared.Primitives;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class WaitDaysTooLessError : IError
{
    private readonly Error _error;

    public WaitDaysTooLessError(PositiveInteger waitDays) =>
        _error = Error.Conflict($"Время должно быть больше {waitDays.Read()}");

    public Error Read() => _error;

    public static implicit operator Status(WaitDaysTooLessError error) => error._error;
}
