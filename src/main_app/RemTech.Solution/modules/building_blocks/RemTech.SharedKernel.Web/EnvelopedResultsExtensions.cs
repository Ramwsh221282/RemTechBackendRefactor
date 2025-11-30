using System.Net;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Web;

public static class EnvelopedResultsExtensions
{
    extension(Result result)
    {
        public Envelope AsEnvelope()
        {
            object? body = null;
            int code = result.ResolveStatusCode();
            string? message = result.ResolveMessage();
            return new Envelope(code, body, message);
        }

        public Envelope AsEnvelope<T>(Func<T> onSuccess)
        {
            T? body = result.IsFailure ? default : onSuccess();
            int code = result.ResolveStatusCode();
            string? message = result.ResolveMessage();
            return new Envelope(code, body, message);
        }

        private int ResolveStatusCode()
        {
            HttpStatusCode code = result.IsSuccess switch
            {
                true => HttpStatusCode.OK,
                false => result.ResolveStatusCodeByError(result.Error) 
            };

            return (int)code;
        }

        private string? ResolveMessage()
        {
            string? message = result.IsFailure ? result.Error.Message : null;
            return message;
        }
        
        private HttpStatusCode ResolveStatusCodeByError(Error error)
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
    }

    extension<T>(Result<T> result)
    {
        public Envelope AsEnvelope()
        {
            object? body = result.ResolveBody();
            int code = result.ResolveStatusCode();
            string? message = result.ResolveMessage();
            return new Envelope(code, body, message);
        }
        
        public Envelope AsEnvelope(Func<T> onSuccess)
        {
            object? body = result.IsFailure ? default : onSuccess();
            int code = result.ResolveStatusCode();
            string? message = result.ResolveMessage();
            return new Envelope(code, body, message);
        }

        private object? ResolveBody()
        {
            object? body = result.IsSuccess ? result.Value : null;
            return body;
        }
    }
}