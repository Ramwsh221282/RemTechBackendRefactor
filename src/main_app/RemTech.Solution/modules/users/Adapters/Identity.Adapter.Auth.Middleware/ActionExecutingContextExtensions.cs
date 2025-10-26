using Identity.Adapter.Jwt;
using Identity.Domain.Sessions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Shared.WebApi;

namespace Identity.Adapter.Auth.Middleware;

internal static class ActionExecutingContextExtensions
{
    internal static UserSession GetUserSession(this ActionExecutingContext context)
    {
        // получение информации о refresh и access токенах из http запроса.
        var accessToken = context.HttpContext.Request.Headers[TokenConstants.AccessToken];
        var refreshToken = context.HttpContext.Request.Headers[TokenConstants.RefreshToken];

        string accessTokenString = HeaderValueOrEmpty(accessToken);
        string refreshTokenString = HeaderValueOrEmpty(refreshToken);

        return new UserSession(
            new UserSessionInfo(accessTokenString),
            new UserSessionInfo(refreshTokenString)
        );
    }

    internal static async Task HandleForbidden(this ActionExecutingContext context)
    {
        context.HttpContext.Response.StatusCode = 403;
        context.HttpContext.Response.ContentType = "application/json";
        var envelope = HttpEnvelope.Forbidden();
        await context.HttpContext.Response.WriteAsJsonAsync(envelope);
    }

    private static string HeaderValueOrEmpty(StringValues value)
    {
        // возвращаем пустую строку или значение строки из хедера.
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.ToString();
    }
}
