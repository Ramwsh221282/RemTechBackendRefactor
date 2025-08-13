namespace RemTech.Bootstrap.Api.Middlewares;

internal sealed class InternalServerErrorMiddleware(RequestDelegate next, Serilog.ILogger logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.Fatal("Middleware: {Ex}", ex);
            context.Response.Clear();
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var message = new { message = "Ошибка на стороне приложения" };
            await context.Response.WriteAsJsonAsync(message);
        }
    }
}
