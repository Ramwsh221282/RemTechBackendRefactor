namespace RemTech.Primitives.Extensions.Exceptions;

public abstract class MessageException(string error) : Exception(error)
{
    public string Error => error;
    public class ConflictException(string error) : MessageException(error);
    public class NotFoundException(string error) : MessageException(error);
    public class ValidationException(string error) : MessageException(error);
    public class InternalException(string error) : MessageException(error);

    public static void Conflict(string message)
    {
        throw new ConflictException(message);
    }
    
    public static void NotFound(string message)
    {
        throw new NotFoundException(message);
    }
    
    public static void Validation(string message)
    {
        throw new ValidationException(message);
    }
    
    public static void Internal(string message)
    {
        throw new InternalException(message);
    }
    
}