namespace RemTech.Core.Shared.Exceptions;

public sealed class ValueNotValidException : Exception
{
    private readonly string _propertyName;
    private readonly string? _erroredProperty;
    private readonly int _errorCode = 400;

    public ValueNotValidException(string propertyName, string? erroredProperty = null)
    {
        _propertyName = propertyName;
        _erroredProperty = erroredProperty;
    }

    public override string Message
    {
        get
        {
            string error = string.IsNullOrWhiteSpace(_erroredProperty)
                ? $"{_propertyName} не может пустым."
                : $"{_propertyName} не может быть {_erroredProperty}.";
            return error;
        }
    }
    
    public int Code() => _errorCode;
}