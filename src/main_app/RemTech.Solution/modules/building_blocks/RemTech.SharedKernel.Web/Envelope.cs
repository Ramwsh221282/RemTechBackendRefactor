using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RemTech.SharedKernel.Web;

public sealed class Envelope : IResult
{
    private int _statusCode { get; }
    private object? _body { get; }
    private string? _error { get; }

    public Envelope(int statusCode, object? body, string? error)
    {
        _statusCode = statusCode;
        _body = body;
        _error = error;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = _statusCode;
        httpContext.Response.ContentType = "application/json; charset=utf-8";
        string jsonBody = Serialized();
        await httpContext.Response.WriteAsync(jsonBody);
    }

    private string Serialized()
    {
        Dictionary<string, object?> data = [];
        data.Add("statusCode", _statusCode);
        data.Add("body", _body);
        data.Add("error", _error);
        return JsonSerializer.Serialize(data);
    }
}