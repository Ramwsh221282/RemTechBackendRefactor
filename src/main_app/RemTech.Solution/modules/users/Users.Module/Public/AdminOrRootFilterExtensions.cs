using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Module.Public;

public static class AdminOrRootFilterExtensions
{
    public static RouteGroupBuilder RequireAdminOrRootAccess(this RouteGroupBuilder group)
    {
        return group.AddEndpointFilter<AdminOrRootAccessFilter>();
    }

    public static RouteHandlerBuilder RequireAdminOrRootAccess(this RouteHandlerBuilder part)
    {
        return part.AddEndpointFilter<AdminOrRootAccessFilter>();
    }
}
