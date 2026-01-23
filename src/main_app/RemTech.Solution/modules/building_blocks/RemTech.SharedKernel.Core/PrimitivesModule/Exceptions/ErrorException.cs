namespace RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

public abstract class ErrorException(string error) : Exception(error)
{
    public string Error { get; } = error;

    public class ConflictException(string error) : ErrorException(error);

    public class NotFoundException(string error) : ErrorException(error);

    public class ValidationException(string error) : ErrorException(error);

    public class InternalException(string error) : ErrorException(error);

    public static ConflictException Conflict(string message) => new(message);

    public static NotFoundException NotFound(string message) => new(message);

    public static ValidationException Validation(string message) => new(message);

    public static ValidationException ValueNotSet(string valueName)
    {
        string message = $"{valueName}. Значение не заполнено.";
        return Validation(message);
    }

    public static ValidationException ValueExcess(string valueName, int length)
    {
        string message = $"{valueName}. Превышает длину {length}.";
        return Validation(message);
    }

    public static ValidationException ValueInvalidFormat(string valueName)
    {
        string message = $"{valueName}. Некорректный формат.";
        return Validation(message);
    }

    public static ValidationException ValueInvalid(string valueName)
    {
        string message = $"{valueName}. Невалидное значение.";
        return Validation(message);
    }

    public static ValidationException ValueInvalid(string valueName, string value)
    {
        string message = $"{valueName}. Невалидное значение {value}.";
        return Validation(message);
    }

    public static InternalException Internal(string message) => new(message);
}
