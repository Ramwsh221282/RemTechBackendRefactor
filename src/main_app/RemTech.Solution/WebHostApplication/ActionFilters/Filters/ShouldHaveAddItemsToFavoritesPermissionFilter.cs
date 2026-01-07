using Identity.Domain.Contracts.Jwt;
using Microsoft.AspNetCore.Mvc.Filters;
using WebHostApplication.ActionFilters.Common;

namespace WebHostApplication.ActionFilters.Filters;

public sealed class ShouldHaveAddItemsToFavoritesPermissionFilter(IJwtTokenManager manager) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.HasPermission("add.items.to.favorites", manager))
        {
            await context.HttpContext.WriteForbiddenResult();
            return;
        }
        
        await next();
    }
}