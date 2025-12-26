namespace RemTech.Core.Shared.Result;

public sealed class ValidationScope
{
    private readonly List<Error> _errors = [];

    public ValidationScope Check(Status status)
    {
        if (status.IsSuccess)
            return this;

        if (status.Error.Code != ErrorCodes.Validation)
            throw new ApplicationException(
                $"Error: {status.Error.ErrorText} is not validation error."
            );

        _errors.Add(status);
        return this;
    }

    public bool Any() => _errors.Any();

    public Error ToError()
    {
        IEnumerable<string> errors = _errors.Select(er => er.ErrorText);
        string finalMessage = string.Join(" ,", errors);
        return Error.Validation(finalMessage);
    }
}
