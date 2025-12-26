namespace RemTech.Core.Shared.Result;

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