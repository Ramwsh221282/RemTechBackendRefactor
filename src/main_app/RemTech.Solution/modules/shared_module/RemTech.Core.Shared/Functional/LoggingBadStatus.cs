using RemTech.Logging.Library;
using RemTech.Result.Library;

namespace RemTech.Core.Shared.Functional;

public class LoggingBadStatus(ICustomLogger logger, Status status)
{
    public Status Logged()
    {
        if (status.IsSuccess)
            throw new ArgumentException("Статус был успешный.");
        logger.Error("Ошибка: {0}.", status.Error.ErrorText);
        return status;
    }
}

public sealed class LoggingBadStatus<T>(ICustomLogger logger, Status<T> status)
    : LoggingBadStatus(logger, status)
{
    public new Status<T> Logged()
    {
        base.Logged();
        return status;
    }
}
