using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RemTech.SharedKernel.Web;

/// <summary>
/// Оболочка для стандартизированного HTTP-ответа.
/// </summary>
/// <param name="statusCode">HTTP статус код ответа.</param>
/// <param name="body">Тело ответа.</param>
/// <param name="message">Сообщение ответа.</param>
public sealed class Envelope(int statusCode, object? body, string? message) : IResult
{
	private static readonly JsonSerializerOptions _jsonOptions = new()
	{
		WriteIndented = true,
		Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
	};

	private int StatusCode { get; } = statusCode;
	private object? Body { get; } = body;
	private string? Message { get; } = message;

	/// <summary>
	/// Выполняет запись стандартизированного HTTP-ответа в контекст HTTP.
	/// </summary>
	/// <param name="httpContext">Контекст HTTP для записи ответа.</param>
	/// <returns>Задача, представляющая асинхронную операцию записи ответа.</returns>
	public Task ExecuteAsync(HttpContext httpContext)
	{
		httpContext.Response.StatusCode = StatusCode;
		httpContext.Response.ContentType = "application/json; charset=utf-8";
		string jsonBody = Serialized();
		return httpContext.Response.WriteAsync(jsonBody);
	}

	private string Serialized()
	{
		Dictionary<string, object?> data = [];
		data.Add("statusCode", StatusCode);
		data.Add("body", Body);
		data.Add("message", Message);
		return JsonSerializer.Serialize(data, _jsonOptions);
	}
}
