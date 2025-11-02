namespace RemTech.Core.Shared.Result;

public record Error(string ErrorText, ErrorCodes Code)
{
    public static Error None() => new(string.Empty, ErrorCodes.Empty);

    public static Error Validation(string errorMessage) => new(errorMessage, ErrorCodes.Validation);

    public static Error NotFound(string errorMessage) => new(errorMessage, ErrorCodes.NotFound);

    public static Error Conflict(string errorMessage) => new(errorMessage, ErrorCodes.Conflict);

    public static Error Conflict(string errorTemplate, params object[] arguments)
    {
        ErrorCodes code = ErrorCodes.Conflict;
        string message = string.Format(errorTemplate, arguments);
        return new Error(message, code);
    }

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

    public static Error Combined(IEnumerable<Error> errors)
    {
        var distinctCodes = errors.Select(er => er.Code).Distinct().ToArray();
        if (distinctCodes.Length > 1)
            throw new ApplicationException("Unable to create combined error. Codes are different.");

        var texts = errors.Select(er => er.ErrorText);
        var text = string.Join(" ,", texts);
        var code = distinctCodes[0];
        return new Error(text, code);
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