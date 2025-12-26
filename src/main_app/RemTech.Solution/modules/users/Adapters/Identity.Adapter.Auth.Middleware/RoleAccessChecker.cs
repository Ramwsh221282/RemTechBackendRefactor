using Microsoft.AspNetCore.Mvc.Filters;
using Shared.WebApi;

namespace Identity.Adapter.Auth.Middleware;

public sealed class RoleAccessChecker : IRoleAccessChecker
{
    public Task<bool> HasAccess(ActionExecutingContext context, IEnumerable<string> roles)
    {
        var session = context.GetUserSession();
        var sessionRoles = session.GetRoleNames();
        bool containsAny = roles.Any(r => sessionRoles.Any(s => s == r));
        return Task.FromResult(containsAny);
    }
}
