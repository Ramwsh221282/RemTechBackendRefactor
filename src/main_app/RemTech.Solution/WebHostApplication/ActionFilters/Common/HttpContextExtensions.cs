using System.Net;
using Identity.Domain.Contracts.Jwt;
using Microsoft.Extensions.Primitives;
using RemTech.SharedKernel.Web;

namespace WebHostApplication.ActionFilters.Common;

public static class HttpContextExtensions
{
    extension(HttpContext context)
    {
        public string GetRefreshTokenOrEmpty() => GetHeaderValueOrEmpty(context, "refresh_token");

        public string GetAccessTokenOrEmpty() => GetHeaderValueOrEmpty(context, "access_token");

        public CancellationToken CancellationToken => context.RequestAborted;
        
        public bool HasPermission(string permission, IJwtTokenManager manager)
        {
            string token = context.GetAccessTokenOrEmpty();
            return manager.ReadToken(token).ContainsPermission(permission);
        }
        
        public async Task WriteForbiddenResult()
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";
            Envelope envelope = ForbiddenEnvelope();
            await context.Response.WriteAsJsonAsync(envelope, context.CancellationToken);
        }
    }
    
    private static Envelope ForbiddenEnvelope()
    {
        return new((int)HttpStatusCode.Forbidden, null, "Forbidden");
    }
    
    private static string GetHeaderValueOrEmpty(HttpContext context, string headerName)
    {
        if (!context.Request.Headers.TryGetValue(headerName, out StringValues refreshToken))
            return string.Empty;
        
        string? token = refreshToken.FirstOrDefault();
        return token ?? string.Empty;
    }
}