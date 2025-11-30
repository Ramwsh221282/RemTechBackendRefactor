using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RemTech.SharedKernel.Web;

public sealed class Envelope : IResult
{
    private int StatusCode { get; }
    private object? Body { get; }
    private string? Error { get; }

    public Envelope(int statusCode, object? body, string? error)
    {
        StatusCode = statusCode;
        Body = body;
        Error = error;
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
        data.Add("error", Error);
        return JsonSerializer.Serialize(data);
    }
}