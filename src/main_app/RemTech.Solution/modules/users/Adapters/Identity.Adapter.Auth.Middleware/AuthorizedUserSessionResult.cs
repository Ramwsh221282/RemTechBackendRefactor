using System.Net;
using Identity.Adapter.Jwt;
using Identity.Domain.Sessions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Identity.Adapter.Auth.Middleware;

public sealed class AuthorizedUserSessionResult(UserSession session) : IResult
{
    private const string AccessTokenHeader = TokenConstants.AccessToken;
    private const string RefreshTokenHeader = TokenConstants.RefreshToken;
    private readonly string _accessTokenValue = session.AccessToken.Token;
    private readonly string _refreshTokenValue = session.GetRefreshToken();

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        // не отдаем содержимое, просто без контента.
        httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        httpContext.Response.ContentType = "text/plain";

        // пишем в хедеры refresh и access токены.
        KeyValuePair<string, StringValues> access = new(AccessTokenHeader, _accessTokenValue);
        KeyValuePair<string, StringValues> refresh = new(RefreshTokenHeader, _refreshTokenValue);

        httpContext.Response.Headers.Add(access);
        httpContext.Response.Headers.Add(refresh);

        // записываем ответ.
        await httpContext.Response.WriteAsync("OK");
    }
}
