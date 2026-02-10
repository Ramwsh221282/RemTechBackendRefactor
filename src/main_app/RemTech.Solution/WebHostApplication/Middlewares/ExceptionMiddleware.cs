using System.Net;
using System.Text.Json;
using RemTech.SharedKernel.Web;

namespace WebHostApplication.Middlewares;

/// <summary>
/// Миддлевейр для обработки исключений в HTTP запросах.
/// </summary>
/// <param name="next">Следующий делегат запроса.</param>
/// <param name="logger">Логгер для записи информации.</param>
public sealed class ExceptionMiddleware(RequestDelegate next, Serilog.ILogger logger)
{
	private readonly RequestDelegate _next = next;
	private readonly Serilog.ILogger _logger = logger.ForContext<ExceptionMiddleware>();

	/// <summary>
	/// Обрабатывает HTTP запрос и перехватывает исключения.
	/// </summary>
	/// <param name="context">Контекст HTTP запроса.    </param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
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
		context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
		context.Response.ContentType = "application/json";
        Dictionary<string, object?> envelope = InternalServerResponseInformation();
		return context.Response.WriteAsJsonAsync(envelope, context.RequestAborted);
	}

    private static Dictionary<string, object?> InternalServerResponseInformation()
    {
        Dictionary<string, object?> response = new();
        response.Add("statusCode", (int)HttpStatusCode.InternalServerError);
        response.Add("message", "Internal server error");
        response.Add("body", null);
        return response;
    }
}
