namespace RemTech.Core.Shared.Result;

public record Error(string ErrorText, ErrorCodes Code)
{
    public static Error None() => new(string.Empty, ErrorCodes.Empty);

    public static Error Validation(string errorMessage) => new(errorMessage, ErrorCodes.Validation);

    public static Error NotFound(string errorMessage) => new(errorMessage, ErrorCodes.NotFound);

    public static Error Conflict(string errorMessage) => new(errorMessage, ErrorCodes.Conflict);

    public static Error PasswordIncorrect() => new("Пароль неверный", ErrorCodes.Unauthorized);

    public static Error TokensExpired() =>
        new Error("Expired tokens sessions.", ErrorCodes.Unauthorized);

    public static Error Forbidden(string message) => new(message, ErrorCodes.Forbidden);

    public static Error Unauthorized() => new("Необходима авторизация.", ErrorCodes.Unauthorized);

    public static Error Forbidden() => new("Доступ к ресурсу запрещен.", ErrorCodes.Forbidden);

    public Status Status() => new(this);

    public Error Combine(Error other)
    {
        if (other.Code != Code)
            throw new ApplicationException(
                $"Uncompatible errors. Right code: {Code}. Left code: {other.Code}"
            );

        string message = string.Join(", ", [ErrorText, other.ErrorText]);
        return new Error(message, Code);
    }

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

    public static implicit operator Task<Status<T>>(ValidationError<T> error)
    {
        return Task.FromResult<Status<T>>(error);
    }

    public static implicit operator Task<Status>(ValidationError<T> error)
    {
        return Task.FromResult<Status>(error);
    }
}
