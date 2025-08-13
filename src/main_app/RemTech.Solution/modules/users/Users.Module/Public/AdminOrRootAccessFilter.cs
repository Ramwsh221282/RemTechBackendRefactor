using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Models;

namespace Users.Module.Public;

public sealed class AdminOrRootAccessFilter(
    ConnectionMultiplexer multiplexer,
    Serilog.ILogger logger,
    SecurityKeySource key
) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        HttpContext httpContext = context.HttpContext;

        try
        {
            if (!HasAccessTokenId(httpContext, out StringValues accessTokenId))
            {
                logger.Warning(
                    "{Context} access denied for request: {Request}. Token id is required.",
                    nameof(AdminOrRootAccessFilter),
                    httpContext.Request.Path.Value
                );
                return Results.Json(
                    new { message = "Access denied." },
                    statusCode: StatusCodes.Status401Unauthorized
                );
            }

            if (!IsAccessTokenGuid(accessTokenId, out Guid tokenGuid))
            {
                logger.Warning(
                    "{Context} access denied for request: {Request}. Token id is invalid format.",
                    nameof(AdminOrRootAccessFilter),
                    httpContext.Request.Path.Value
                );
                return Results.Json(
                    new { message = "Access denied." },
                    statusCode: StatusCodes.Status401Unauthorized
                );
            }

            if (await IsTokenOfAdminOrRoot(tokenGuid))
                return await next(context);

            logger.Warning(
                "{Context} access denied for request: {Request}. Token neither of ROOT or ADMIN.",
                nameof(AdminOrRootAccessFilter),
                httpContext.Request.Path.Value
            );
            return Results.StatusCode(403);
        }
        catch (Exception ex)
        {
            logger.Error("{Entrance} {Message}", nameof(AdminOrRootAccessFilter), ex.Message);
            return Results.StatusCode(403);
        }
    }

    private async Task<bool> IsTokenOfAdminOrRoot(Guid tokenId)
    {
        UserJwt jwt = new UserJwt(tokenId);
        jwt = await jwt.Provide(multiplexer);
        jwt = await jwt.CheckSubscription(key, multiplexer);
        return jwt.IsOfRole("ADMIN") || jwt.IsOfRole("ROOT");
    }

    private static bool IsAccessTokenGuid(StringValues accessTokenIdValue, out Guid guidToken)
    {
        bool isGuid = Guid.TryParse(accessTokenIdValue, out Guid tokenGuid);
        guidToken = tokenGuid;
        return isGuid;
    }

    private static bool HasAccessTokenId(HttpContext context, out StringValues accessTokenIdValue)
    {
        bool has = context.Request.Headers.TryGetValue(
            "RemTechAccessTokenId",
            out StringValues accessTokenId
        );
        accessTokenIdValue = accessTokenId;
        return has;
    }
}
