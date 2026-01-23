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

    public static Envelope AsEnvelope<T>(T result)
    {
        object? body = result;
        const int code = (int)HttpStatusCode.OK;
        const string? message = null;
        return new Envelope(code, body, message);
    }

    public static Envelope AsEnvelope<T, U>(T result, Func<T, U> converter)
    {
        U body = converter(result);
        const int code = (int)HttpStatusCode.OK;
        const string? message = null;
        return new Envelope(code, body, message);
    }

    extension(Result result)
    {
        private int ResolveStatusCode()
        {
            HttpStatusCode code = result.IsSuccess switch
            {
                true => HttpStatusCode.OK,
                false => ResolveStatusCodeByError(result.Error),
            };

            return (int)code;
        }

        private string? ResolveMessage() => result.IsFailure ? result.Error.Message : null;

        private static HttpStatusCode ResolveStatusCodeByError(Error error) =>
            error switch
            {
                Error.ForbiddenError => HttpStatusCode.Forbidden,
                Error.UnauthorizedError => HttpStatusCode.Unauthorized,
                Error.ApplicationError => HttpStatusCode.InternalServerError,
                Error.ConflictError => HttpStatusCode.Conflict,
                Error.NotFoundError => HttpStatusCode.NotFound,
                Error.ValidationError => HttpStatusCode.BadRequest,
                Error.NoneError => throw new InvalidOperationException(
                    "None error cannot be resolved to http status code."
                ),
                _ => throw new InvalidOperationException(
                    $"Unknown error type cannot be resolved to http status code. Error type: {error.GetType().Name}"
                ),
            };
    }

    extension(Result result)
    {
        public Envelope AsEnvelope()
        {
            if (result.IsFailure)
            {
                object? body = null;
                string error = result.Error.Message;
                HttpStatusCode code = ResolveStatusCodeByError(result.Error);
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

        public Envelope AsTypedEnvelope<U>(Func<U> onSuccess)
        {
            U? body = result.IsSuccess ? onSuccess() : default;
            int code = result.ResolveStatusCode();
            string? message = result.ResolveMessage();
            return new Envelope(code, body, message);
        }
    }

    extension<T>(Result<T> result)
    {
        public Envelope AsTypedEnvelope(Func<T> onSuccess)
        {
            object? body = result.IsSuccess ? onSuccess() : null;
            int code = result.ResolveStatusCode();
            string? message = result.ResolveMessage();
            return new Envelope(code, body, message);
        }

        public Envelope AsTypedEnvelope<U>(Func<T, U> onSuccess)
        {
            object? body = result.IsSuccess ? onSuccess(result.Value) : null;
            int code = result.ResolveStatusCode();
            string? message = result.ResolveMessage();
            return new Envelope(code, body, message);
        }
    }
}
