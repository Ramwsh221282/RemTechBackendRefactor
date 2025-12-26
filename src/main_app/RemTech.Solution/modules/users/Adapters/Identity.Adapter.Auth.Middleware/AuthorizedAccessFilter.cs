using Identity.Adapter.Jwt;
using Identity.Domain.Sessions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using RemTech.Core.Shared.Result;
using Shared.WebApi;

namespace Identity.Adapter.Auth.Middleware;

public sealed class AuthorizedAccessFilter(UserSessionsService service) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        var session = context.GetUserSession();
        if (!await IsSessionVerified(session))
        {
            var refreshing = await TryRefreshUserSession(session);
            if (refreshing.IsFailure)
            {
                await ReturnUnauthorized(context);
                return;
            }

            await HandleAsRefreshedSession(refreshing, context, next);
            return;
        }

        await next();
    }

    private async Task<bool> IsSessionVerified(UserSession session)
    {
        return await service.Validate(session);
    }

    private async Task HandleAsRefreshedSession(
        UserSession session,
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        // выполнение запроса.
        await next();

        // удаление старых заголовков.
        context.HttpContext.Response.Headers.Remove(TokenConstants.AccessToken);
        context.HttpContext.Response.Headers.Remove(TokenConstants.RefreshToken);

        string accessToken = session.AccessToken.Token;
        string refreshToken = session.GetRefreshToken();

        var accessTokenHeader = new KeyValuePair<string, StringValues>(
            TokenConstants.AccessToken,
            accessToken
        );

        var refreshTokenHeader = new KeyValuePair<string, StringValues>(
            TokenConstants.RefreshToken,
            refreshToken
        );

        // добавление новых заголовков.
        context.HttpContext.Response.Headers.Add(accessTokenHeader);
        context.HttpContext.Response.Headers.Add(refreshTokenHeader);
    }

    private async Task ReturnUnauthorized(ActionExecutingContext context)
    {
        // удаление заголовков (если они есть - удалятся).
        context.HttpContext.Response.Headers.Remove(TokenConstants.AccessToken);
        context.HttpContext.Response.Headers.Remove(TokenConstants.RefreshToken);

        // наполнение ответа от сервиса неавторизованным результатом.
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.HttpContext.Response.WriteAsJsonAsync(HttpEnvelope.Unauthorized());
    }

    private async Task<Status<UserSession>> TryRefreshUserSession(UserSession session)
    {
        // попытка обновить сессию через refresh token.
        var refreshed = await service.Refresh(session);
        if (refreshed.IsFailure)
            return Error.Unauthorized();

        bool verified = await service.Validate(refreshed);
        return verified ? refreshed : Error.Unauthorized();
    }
}
