using System.Net;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Web;

public static class EnvelopedResultsExtensions
{
    public static Envelope AsEnvelope<T>(this Result<T> result, Func<T> onSuccess)
    {
        T? body = result.IsFailure ? default : onSuccess();
        int code = result.ResolveStatusCode();
        string? message = result.ResolveMessage();
        return new Envelope(code, body, message);
    }

    private static int ResolveStatusCode(this Result result)
    {
        HttpStatusCode code = result.IsSuccess switch
        {
            true => HttpStatusCode.OK,
            false => result.ResolveStatusCodeByError(result.Error) 
        };

        return (int)code;
    }

    private static string? ResolveMessage(this Result result)
    {
        string? message = result.IsFailure ? result.Error.Message : null;
        return message;
    }
        
    private static HttpStatusCode ResolveStatusCodeByError(this Result result, Error error)
    {
        return error switch
        {
            Error.ApplicationError => HttpStatusCode.InternalServerError,
            Error.ConflictError => HttpStatusCode.Conflict,
            Error.NotFoundError => HttpStatusCode.NotFound,
            Error.ValidationError => HttpStatusCode.BadRequest,
            Error.NoneError => throw new InvalidOperationException("None error cannot be resolved to http status code."),
            _ => throw new InvalidOperationException($"Unknown error type cannot be resolved to http status code. Error type: {error.GetType().Name}")
        };
    }
        
    public static Envelope AsEnvelope(this Result result)
    {
        if (result.IsFailure)
        {
            object? body = null;
            string error = result.Error.Message;
            HttpStatusCode code = result.ResolveStatusCodeByError(result.Error);
            return new Envelope((int)code, body, error);
        }
        else
        {
            object? body = null;
            int code = result.ResolveStatusCode();
            string? message = result.ResolveMessage();
            return new Envelope(code, body, message);
        }
    }
     
    public static Envelope AsTypedEnvelope<U>(this Result result, Func<U> onSuccess)
    {
        U? body = result.IsSuccess ? onSuccess() : default;
        int code = result.ResolveStatusCode();
        string? message = result.ResolveMessage();
        return new Envelope(code, body, message);
    }
    
    public static Envelope AsTypedEnvelope<T>(this Result<T> result, Func<T> onSuccess)
    {
        object? body = result.IsSuccess ? onSuccess() : null;
        int code = result.ResolveStatusCode();
        string? message = result.ResolveMessage();
        return new Envelope(code, body, message);
    }
    
    public static Envelope AsTypedEnvelope<T,U>(this Result<T> result, Func<T,U> onSuccess)
    {
        object? body = result.IsSuccess ? onSuccess(result.Value) : null;
        int code = result.ResolveStatusCode();
        string? message = result.ResolveMessage();
        return new Envelope(code, body, message);
    }
}