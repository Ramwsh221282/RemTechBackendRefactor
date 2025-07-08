using RemTech.Core.Shared.Primitives;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public class WaitDaysTooMuchError : IError
{
    private readonly Error _error;

    public WaitDaysTooMuchError(PositiveInteger waitDays) =>
        _error = Error.Conflict($"Время должно быть меньше {waitDays.Read()}");

    public Error Read() => _error;

    public static implicit operator Status(WaitDaysTooMuchError error) => error._error;
}
