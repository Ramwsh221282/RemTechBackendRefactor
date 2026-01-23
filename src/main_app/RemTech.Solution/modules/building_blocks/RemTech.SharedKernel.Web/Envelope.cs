using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RemTech.SharedKernel.Web;

public sealed class Envelope : IResult
{
	private int StatusCode { get; }
	private object? Body { get; }
	private string? Message { get; }

	public Envelope(int statusCode, object? body, string? message)
	{
		StatusCode = statusCode;
		Body = body;
		Message = message;
	}

	public async Task ExecuteAsync(HttpContext httpContext)
	{
		httpContext.Response.StatusCode = StatusCode;
		httpContext.Response.ContentType = "application/json; charset=utf-8";
		string jsonBody = Serialized();
		await httpContext.Response.WriteAsync(jsonBody);
	}

	private string Serialized()
	{
		Dictionary<string, object?> data = [];
		data.Add("statusCode", StatusCode);
		data.Add("body", Body);
		data.Add("message", Message);
		return JsonSerializer.Serialize(data);
	}
}
