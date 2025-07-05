namespace RemTech.Result.Library;

public sealed record Error(string ErrorText, ErrorCodes Code)
{
    public static Error None() => new(string.Empty, ErrorCodes.Empty);

    public static Error Validation(string errorMessage) => new(errorMessage, ErrorCodes.Validation);

    public static Error NotFound(string errorMessage) => new(errorMessage, ErrorCodes.NotFound);

    public static Error Conflict(string errorMessage) => new(errorMessage, ErrorCodes.Conflict);

    public bool Any() => !(string.IsNullOrWhiteSpace(ErrorText) || Code == ErrorCodes.Empty);
}
