namespace RemTech.Result.Library;

public record Error(string ErrorText, ErrorCodes Code)
{
    public static Error None() => new(string.Empty, ErrorCodes.Empty);

    public static Error Validation(string errorMessage) => new(errorMessage, ErrorCodes.Validation);

    public static Error NotFound(string errorMessage) => new(errorMessage, ErrorCodes.NotFound);

    public static Error Conflict(string errorMessage) => new(errorMessage, ErrorCodes.Conflict);

    public Status Status() => new(this);

    public Status<T> Status<T>() => new(this);

    public bool Any() => !(string.IsNullOrWhiteSpace(ErrorText) || Code == ErrorCodes.Empty);

    public static implicit operator Error((string, ErrorCodes code) tuple) =>
        new(tuple.Item1, tuple.code);

    public static implicit operator Status(Error error)
    {
        return error.Status();
    }
}

public sealed record Error<T>(string ErrorText, ErrorCodes Code) : Error(ErrorText, Code)
{
    public static implicit operator Status<T>(Error<T> error)
    {
        Error upcated = error;
        return new Status<T>(upcated);
    }

    public static implicit operator Status(Error<T> error)
    {
        Error upcated = error;
        return new Status(upcated);
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

    public Status Status() => new(this);

    public Status<T> Status<T>() => new(this);

    public static implicit operator Error(ValidationError validationError)
    {
        return new Error(validationError._text, validationError._code);
    }

    public static implicit operator Status(ValidationError validationError)
    {
        return validationError.Status();
    }
}

public sealed record ValidationError<T> : ValidationError
{
    public ValidationError(string text)
        : base(text) { }

    public static implicit operator Status<T>(ValidationError<T> validationError)
    {
        ValidationError upcated = validationError;
        return new Status<T>(upcated);
    }

    public static implicit operator Status(ValidationError<T> validationError)
    {
        ValidationError upcated = validationError;
        return new Status(upcated);
    }
}
