using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Ports.Storage;
using Microsoft.AspNetCore.Mvc.Filters;
using RemTech.Core.Shared.Result;
using Shared.WebApi;

namespace Identity.WebApi.Filters;

public sealed class FirstRootFilter : IAsyncActionFilter
{
    private readonly IUsersStorage _users;

    public FirstRootFilter(IUsersStorage user)
    {
        _users = user;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        RoleName root = RoleName.Root;
        var users = await _users.Get(root);
        if (users.Any())
        {
            await HandleAsConflictError(context);
            return;
        }

        await next();
    }

    private async Task HandleAsConflictError(ActionExecutingContext context)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        context.HttpContext.Response.ContentType = "application/json";
        var status = Status.Conflict("First root user already exists.");
        var envelope = new HttpEnvelope(status);
        await context.HttpContext.Response.WriteAsJsonAsync(envelope);
    }
}
