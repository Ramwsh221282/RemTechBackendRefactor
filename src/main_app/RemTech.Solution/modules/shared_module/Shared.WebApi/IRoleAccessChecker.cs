using Microsoft.AspNetCore.Mvc.Filters;

namespace Shared.WebApi;

public interface IRoleAccessChecker
{
    Task<bool> HasAccess(ActionExecutingContext context, IEnumerable<string> roles);
}