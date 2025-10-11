using RemTech.Core.Shared.Result;
using Serilog;

namespace RemTech.UseCases.Shared.Logging;

public static class LoggingExtensions
{
    public static Error LoggedError(this ILogger logger, Error error, string? operationName = null)
    {
        logger.Error("{Operation} error: {Error}.", error.ErrorText, operationName);
        return error;
    }

    public static Status LoggedError(
        this ILogger logger,
        Status status,
        string? operationName = null
    )
    {
        Error error = logger.LoggedError(status.Error, operationName);
        return Status.Failure(error);
    }

    public static Status<T> LoggedError<T>(
        this ILogger logger,
        Status<T> status,
        string? operationName = null
    )
    {
        Status parent = status;
        Status logged = logger.LoggedError(parent, operationName);
        return Status<T>.Failure(logged.Error);
    }
}
