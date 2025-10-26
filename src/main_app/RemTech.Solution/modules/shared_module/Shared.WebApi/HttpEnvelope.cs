using System.Net;
using Microsoft.AspNetCore.Http;
using RemTech.Core.Shared.Result;

namespace Shared.WebApi;

public sealed class HttpEnvelope : IResult
{
    public object? Result { get; }
    public string? Error { get; }
    public int StatusCode { get; }

    private HttpEnvelope(object? result, string? error, HttpStatusCode code)
    {
        Result = result;
        Error = error;
        StatusCode = (int)code;
    }

    public HttpEnvelope(string error, int statusCode) => (Error, StatusCode) = (error, statusCode);

    public HttpEnvelope(object result, int statusCode) =>
        (Result, StatusCode) = (result, statusCode);

    public HttpEnvelope(string error, HttpStatusCode statusCode) =>
        (Error, StatusCode) = (error, (int)statusCode);

    public HttpEnvelope(object result, HttpStatusCode statusCode) =>
        (Result, StatusCode) = (result, (int)statusCode);

    public HttpEnvelope(Status status)
    {
        StatusCode = (int)CodeFromErrorCodeStatus(status.Error.Code);
        Error = ErrorTextFromStatus(status);
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(this);
    }

    private static string? ErrorTextFromStatus(Status status)
    {
        if (status.IsSuccess)
            return null;

        string statusError = status.Error.ErrorText;
        return string.IsNullOrWhiteSpace(statusError) ? null : statusError;
    }

    private static HttpStatusCode CodeFromErrorCodeStatus(ErrorCodes code) =>
        code switch
        {
            ErrorCodes.Conflict => HttpStatusCode.Conflict,
            ErrorCodes.Forbidden => HttpStatusCode.Forbidden,
            ErrorCodes.Internal => HttpStatusCode.InternalServerError,
            ErrorCodes.NotFound => HttpStatusCode.NotFound,
            ErrorCodes.Unauthorized => HttpStatusCode.Unauthorized,
            ErrorCodes.Validation => HttpStatusCode.BadRequest,
            ErrorCodes.Empty => HttpStatusCode.NoContent,
            _ => HttpStatusCode.InternalServerError,
        };

    public static HttpEnvelope NoContent()
    {
        return new HttpEnvelope(null, null, HttpStatusCode.NoContent);
    }

    public static HttpEnvelope Conflict()
    {
        return new HttpEnvelope(null, null, HttpStatusCode.Conflict);
    }

    public static HttpEnvelope BadRequest()
    {
        return new HttpEnvelope(null, null, HttpStatusCode.BadRequest);
    }

    public static HttpEnvelope Unauthorized()
    {
        Status status = Status.Unauthorized();
        return new HttpEnvelope(status);
    }

    public static HttpEnvelope Forbidden()
    {
        Status status = Status.Forbidden();
        return new HttpEnvelope(status);
    }

    public static HttpEnvelope Ok()
    {
        return new HttpEnvelope(null, null, HttpStatusCode.OK);
    }

    public static HttpEnvelope Ok(object result)
    {
        return new HttpEnvelope(result, null, HttpStatusCode.OK);
    }
}

public sealed class HttpEnvelope<T> : IResult
    where T : notnull
{
    public T? Result { get; }
    public string? Error { get; }
    public int StatusCode { get; }

    private HttpEnvelope(T result, string? error, HttpStatusCode code)
    {
        Result = result;
        Error = error;
        StatusCode = (int)code;
    }

    public HttpEnvelope(string error, int statusCode) => (Error, StatusCode) = (error, statusCode);

    public HttpEnvelope(T result, int statusCode) => (Result, StatusCode) = (result, statusCode);

    public HttpEnvelope(string error, HttpStatusCode statusCode) =>
        (Error, StatusCode) = (error, (int)statusCode);

    public HttpEnvelope(T result, HttpStatusCode statusCode) =>
        (Result, StatusCode) = (result, (int)statusCode);

    public HttpEnvelope(Status status)
    {
        StatusCode = (int)CodeFromErrorCodeStatus(status.Error.Code);
        Error = ErrorTextFromStatus(status);
    }

    public HttpEnvelope(Status<T> status)
    {
        Status upcasted = status;
        StatusCode = (int)CodeFromErrorCodeStatus(upcasted.Error.Code);
        Error = ErrorTextFromStatus(upcasted);
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(this);
    }

    private static string? ErrorTextFromStatus(Status status)
    {
        if (status.IsSuccess)
            return null;

        string statusError = status.Error.ErrorText;
        return string.IsNullOrWhiteSpace(statusError) ? null : statusError;
    }

    private static HttpStatusCode CodeFromErrorCodeStatus(ErrorCodes code) =>
        code switch
        {
            ErrorCodes.Conflict => HttpStatusCode.Conflict,
            ErrorCodes.Forbidden => HttpStatusCode.Forbidden,
            ErrorCodes.Internal => HttpStatusCode.InternalServerError,
            ErrorCodes.NotFound => HttpStatusCode.NotFound,
            ErrorCodes.Unauthorized => HttpStatusCode.Unauthorized,
            ErrorCodes.Validation => HttpStatusCode.BadRequest,
            ErrorCodes.Empty => HttpStatusCode.NoContent,
            _ => HttpStatusCode.InternalServerError,
        };

    public static HttpEnvelope<T> Ok(T result)
    {
        return new HttpEnvelope<T>(result, null, HttpStatusCode.OK);
    }

    public static HttpEnvelope<T> Unauthorized()
    {
        Status status = Status.Unauthorized();
        return new HttpEnvelope<T>(status);
    }

    public static HttpEnvelope<T> Forbidden()
    {
        Status status = Status.Forbidden();
        return new HttpEnvelope<T>(status);
    }

    public static HttpEnvelope<T> NoContent()
    {
        return new HttpEnvelope<T>(default!, null, HttpStatusCode.NoContent);
    }
}
