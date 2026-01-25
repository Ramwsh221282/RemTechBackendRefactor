using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RemTech.SharedKernel.Web;

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
