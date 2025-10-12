using RemTech.Result.Pattern;
using Serilog;

namespace RemTech.UseCases.Shared.Logging;

public static class LoggingExtensions
{
    public static Result.Pattern.Error LoggedError(
        this ILogger logger,
        Result.Pattern.Error error,
        string? operationName = null
    )
    {
        logger.Error("{Operation} error: {Error}.", error.ErrorText, operationName);
        return error;
    }

    public static Result.Pattern.Result LoggedError(
        this ILogger logger,
        Result.Pattern.Result result,
        string? operationName = null
    )
    {
        Error error = logger.LoggedError(result.Error, operationName);
        return Result.Pattern.Result.Failure(error);
    }

    public static Result<T> LoggedError<T>(
        this ILogger logger,
        Result<T> result,
        string? operationName = null
    )
    {
        Result.Pattern.Result parent = result;
        Result.Pattern.Result logged = logger.LoggedError(parent, operationName);
        return Result<T>.Failure(logged.Error);
    }
}
