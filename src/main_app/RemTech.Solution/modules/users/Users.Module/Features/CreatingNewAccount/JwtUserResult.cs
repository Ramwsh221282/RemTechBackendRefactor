using Microsoft.AspNetCore.Http;

namespace Users.Module.Features.CreatingNewAccount;

internal sealed class JwtUserResult(string tokenId, string tokenValue) : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.Headers.Remove("Authorization");
        httpContext.Response.Headers.Add("Authorization_Token_Id", tokenId);
        httpContext.Response.Headers.Add("Authorization_Token_Value", tokenValue);
        httpContext.Response.StatusCode = 200;
        await httpContext.Response.WriteAsync("{}");
    }
}
