using Identity.Domain.Contracts.Jwt;
using Microsoft.AspNetCore.Mvc.Filters;
using WebHostApplication.ActionFilters.Common;

namespace WebHostApplication.ActionFilters.Filters;

public sealed class ShouldHaveAccessTelemetryPermissionFilter(IJwtTokenManager manager) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.HasPermission("access.telemetry", manager))
        {
            await context.HttpContext.WriteForbiddenResult();
            return;
        }
        
        await next();
    }
}