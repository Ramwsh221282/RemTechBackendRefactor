namespace RemTech.Core.Shared.Exceptions;

public sealed class OperationException : Exception
{
    private readonly string _message;
    private readonly int _code = 400;

    public OperationException(string message)
    {
        _message = message;
    }

    public override string Message => _message;
    public int Code() => _code;
}