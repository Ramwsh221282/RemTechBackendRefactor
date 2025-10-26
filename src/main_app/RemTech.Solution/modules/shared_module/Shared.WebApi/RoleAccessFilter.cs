using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shared.WebApi;

public sealed class RoleAccessFilter(IRoleAccessChecker checker, IEnumerable<string> roles)
    : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        if (!await checker.HasAccess(context, roles))
        {
            context.HttpContext.Response.StatusCode = 403;
            context.HttpContext.Response.ContentType = "application/json";
            var envelope = HttpEnvelope.Forbidden();
            await context.HttpContext.Response.WriteAsJsonAsync(envelope);
            return;
        }

        await next();
    }
}
