namespace RemTech.Functional.Extensions;

public abstract class Error(string message)
{
    public string Message => message;

    public static Error Validation(string message)
    {
        return new ValidationError(message);
    }
    
    public static Error Validation(Result result)
    {
        return new ValidationError(result.Error.Message);
    }

    public static Error Application(string message)
    {
        return new ApplicationError(message);
    }

    public static Error NotFound(string message)
    {
        return new NotFoundError(message);
    }

    public static Error Unauthorized(string message)
    {
        return new UnauthorizedError(message);
    }

    public static Error Forbidden(string message)
    {
        return new ForbiddenError(message);
    }
    
    public static Error Conflict(string message)
    {
        return new ConflictError(message);
    }

    public static Error NoError()
    {
        return new NoneError();
    }

    public sealed class ForbiddenError : Error
    {
        internal ForbiddenError(string message) : base(message)
        {
            
        } 
    }
    
    public sealed class UnauthorizedError : Error
    {
        internal UnauthorizedError(string message) : base(message)
        {
            
        }
    }
    
    public sealed class ValidationError : Error
    {
        internal ValidationError(string message) : base(message)
        {
        }
    }

    public sealed class ApplicationError : Error
    {
        internal ApplicationError(string message) : base(message)
        {
        }
    }

    public sealed class NotFoundError : Error
    {
        internal NotFoundError(string message) : base(message)
        {
        }
    }

    public sealed class ConflictError : Error
    {
        internal ConflictError(string message) : base(message)
        {
        }
    }

    public sealed class NoneError : Error
    {
        internal NoneError() : base(string.Empty)
        {
        }
    }

    public static implicit operator Result(Error error)
    {
        return Result.Failure(error);
    }
}