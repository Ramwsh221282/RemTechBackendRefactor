using Microsoft.AspNetCore.Mvc.Filters;

namespace WebHostApplication.ActionFilters.Common;

public static class ActionExecutingContextExtensions
{
    extension(ActionExecutingContext context)
    {
        public string AccessToken => context.HttpContext.GetAccessTokenOrEmpty();
        public string RefreshToken => context.HttpContext.GetRefreshTokenOrEmpty();
    }
}