using System.Net;
using Identity.Domain.Contracts.Jwt;
using Microsoft.Extensions.Primitives;
using RemTech.SharedKernel.Web;

namespace WebHostApplication.ActionFilters.Common;

public static class HttpContextExtensions
{
    extension(HttpContext context)
    {
        public string GetRefreshTokenFromHeaderOrEmpty() => GetHeaderValueOrEmpty(context, "refresh_token");
        public string GetAccessTokenFromHeaderOrEmpty() => GetHeaderValueOrEmpty(context, "access_token");
        public string GetRefreshTokenFromCookieOrEmpty() => GetCookieValueOrEmpty(context, "refresh_token");
        public string GetAccessTokenFromCookieOrEmpty() => GetCookieValueOrEmpty(context, "access_token");

        public string GetAccessToken(Func<HttpContext, string>[] accessTokenSearchMethods)
        {
            string accessToken = string.Empty;
            foreach (Func<HttpContext, string> method in accessTokenSearchMethods)
            {
                string result = method(context);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    accessToken = result;
                    break;
                }
            }
            return accessToken;
        }

        public string GetRefreshToken(Func<HttpContext, string>[] refreshTokenSearchMethods)
        {
            string refreshToken = string.Empty;
            foreach (Func<HttpContext, string> method in refreshTokenSearchMethods)
            {
                string result = method(context);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    refreshToken = result;
                    break;
                }
            }
            return refreshToken;
        }
        
        public (string AccessToken, string RefreshToken) GetIdentityTokens(
            Func<HttpContext, string>[] accessTokenSearchMethods,
            Func<HttpContext, string>[] refreshTokenSearchMethods
            )
        {
            Dictionary<string, string> tokens = new() { ["access_token"] = string.Empty, ["refresh_token"] = string.Empty };
            
            foreach (Func<HttpContext, string> method in accessTokenSearchMethods)
            {
                string result = method(context);
                if (string.IsNullOrWhiteSpace(result)) continue;
                tokens["access_token"] = result;
                break;
            }
            
            foreach (Func<HttpContext, string> method in refreshTokenSearchMethods)
            {
                string result = method(context);
                if (string.IsNullOrWhiteSpace(result)) continue;
                tokens["refresh_token"] = result;
                break;
            }
            
            return (tokens["access_token"], tokens["refresh_token"]);
        }

        public CancellationToken CancellationToken => context.RequestAborted;
        
        public bool HasPermission(string permission, IJwtTokenManager manager)
        {
            string token = context.GetAccessTokenFromHeaderOrEmpty();
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

    private static string GetCookieValueOrEmpty(HttpContext context, string cookieName)
    {
        if (!context.Request.Cookies.TryGetValue(cookieName, out string? cookieValue))
            return string.Empty;
        return cookieValue;
    }
    
    private static string GetHeaderValueOrEmpty(HttpContext context, string headerName)
    {
        if (!context.Request.Headers.TryGetValue(headerName, out StringValues refreshToken))
            return string.Empty;
        
        string? token = refreshToken.FirstOrDefault();
        return token ?? string.Empty;
    }
}