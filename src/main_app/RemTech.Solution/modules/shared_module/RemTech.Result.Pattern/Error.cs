namespace RemTech.Result.Pattern;

public record Error(string ErrorText, ErrorCodes Code)
{
    public static Error None() => new(string.Empty, ErrorCodes.Empty);

    public static Error Validation(string errorMessage) => new(errorMessage, ErrorCodes.Validation);

    public static Error NotFound(string errorMessage) => new(errorMessage, ErrorCodes.NotFound);

    public static Error Conflict(string errorMessage) => new(errorMessage, ErrorCodes.Conflict);

    public static Error Internal() => new("Ошибка на уровне приложения.", ErrorCodes.Internal);

    public Result Status() => new(this);

    public Result<T> Status<T>() => new(this);

    public bool Any() => !(string.IsNullOrWhiteSpace(ErrorText) || Code == ErrorCodes.Empty);

    public static implicit operator Error((string, ErrorCodes code) tuple) =>
        new(tuple.Item1, tuple.code);

    public static implicit operator Result(Error error)
    {
        return error.Status();
    }
}

public sealed record Error<T>(string ErrorText, ErrorCodes Code) : Error(ErrorText, Code)
{
    public static implicit operator Result<T>(Error<T> error)
    {
        Error upcated = error;
        return new Result<T>(upcated);
    }

    public static implicit operator Result(Error<T> error)
    {
        Error upcated = error;
        return new Result(upcated);
    }
}

public record ValidationError
{
    private readonly string _text;
    private readonly ErrorCodes _code;

    public ValidationError(string text)
    {
        _text = text;
        _code = ErrorCodes.Validation;
    }

    public Result Status() => new(this);

    public Result<T> Status<T>() => new(this);

    public static implicit operator Error(ValidationError validationError)
    {
        return new Error(validationError._text, validationError._code);
    }

    public static implicit operator Result(ValidationError validationError)
    {
        return validationError.Status();
    }
}

public sealed record ValidationError<T> : ValidationError
{
    public ValidationError(string text)
        : base(text) { }

    public static implicit operator Result<T>(ValidationError<T> validationError)
    {
        ValidationError upcated = validationError;
        return new Result<T>(upcated);
    }

    public static implicit operator Result(ValidationError<T> validationError)
    {
        ValidationError upcated = validationError;
        return new Result(upcated);
    }

    public static implicit operator Task<Result<T>>(ValidationError<T> error)
    {
        return Task.FromResult<Result<T>>(error);
    }

    public static implicit operator Task<Result>(ValidationError<T> error)
    {
        return Task.FromResult<Result>(error);
    }
}
