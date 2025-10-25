using Identity.Adapter.Auth.Middleware;
using Identity.Adapter.Jwt;
using Identity.Domain.Sessions;
using Identity.Domain.Users.UseCases.Authenticate;
using Microsoft.AspNetCore.Mvc;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.WebApi;

namespace Identity.WebApi.Controllers.Testing;

[ApiController]
[Route("api/authorization-test")]
public sealed class AuthorizationTestsController : ControllerBase
{
    [HttpGet]
    public async Task<IResult> TestAuth(
        [FromHeader(Name = TokenConstants.AccessToken)] string accessToken,
        [FromHeader(Name = TokenConstants.RefreshToken)] string refreshToken,
        [FromServices] UserSessionsService service
    )
    {
        var session = new UserSession(accessToken, refreshToken);
        bool valid = await service.Validate(session);
        return !valid ? HttpEnvelope.Unauthorized() : HttpEnvelope.NoContent();
    }

    [HttpPost]
    public async Task<IResult> Authorize(
        [FromHeader(Name = "Email")] string? email,
        [FromHeader(Name = "Login")] string? login,
        [FromHeader(Name = "Password")] string password,
        [FromServices] ICommandHandler<AuthenticateCommand, Status<UserSession>> handler,
        CancellationToken ct
    )
    {
        var command = new AuthenticateCommand(login, email, password);
        var status = await handler.Handle(command, ct);

        return status.IsFailure
            ? HttpEnvelope.Unauthorized()
            : new AuthorizedUserSessionResult(status);
    }
}
