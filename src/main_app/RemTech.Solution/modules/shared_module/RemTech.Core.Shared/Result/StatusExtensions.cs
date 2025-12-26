namespace RemTech.Core.Shared.Result;

public static class StatusExtensions
{
    public static Status<TResult> Success<TResult>(this TResult result)
        where TResult : notnull => Status<TResult>.Success(result);

    public static Status<TResult> Select<T, TResult>(this Status<T> status, Func<T, TResult> mapper) =>
        status.IsFailure ? Status<TResult>.Failure(status.Error) : Status<TResult>.Success(mapper(status.Value));

    public static Status<T> Where<T>(this Status<T> status, Func<Status<T>, bool> mapper)
    {
        if (mapper(status))
            return status;
        Error error = status.Error;
        return Status<T>.Failure(error);
    }

    public static Status<T> WithErrorLog<T>(
        this Status<T> status,
        string messageTemplate,
        object[] args,
        Serilog.ILogger? logger = null)
    {
        if (status.IsFailure && logger != null)
        {
            logger.Error(messageTemplate, args);
            return Status<T>.Failure(status);
        }

        return status;
    }

    public static Status<T> WithSuccessLog<T>(
        this Status<T> status,
        string messageTemplate,
        object[] args,
        Serilog.ILogger? logger = null)
    {
        if (status.IsSuccess && logger != null)
        {
            logger.Information(messageTemplate, args);
        }

        return status;
    }

    public static Status<T> OverrideValidationError<T>(this Status<T> status, string message)
    {
        if (status.IsSuccess)
            return status;
        Error error = Error.Validation(message);
        return Status<T>.Failure(error);
    }

    public static Status<T> AsSuccessStatus<T>(this T @object)
    {
        return Status<T>.Success(@object);
    }
}