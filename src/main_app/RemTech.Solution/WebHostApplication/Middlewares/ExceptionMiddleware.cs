using System.Net;
using RemTech.SharedKernel.Web;

namespace WebHostApplication.Middlewares;

public sealed class ExceptionMiddleware(RequestDelegate next, Serilog.ILogger logger)
{
    private readonly RequestDelegate _next = next;
    private readonly Serilog.ILogger _logger = logger.ForContext<ExceptionMiddleware>();

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "At endpoint: {Endpoint}", context.Request.Path);
            await HandleException(context);
        }
    }

    private static Task HandleException(HttpContext context)
    {
        Envelope envelope = new((int)HttpStatusCode.InternalServerError, null, "Internal server error");
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Request.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(envelope, context.RequestAborted);
    }
}
