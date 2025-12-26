using Identity.Domain.Sessions;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.UseCases.Common;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Authenticate;

public sealed class AuthenticationCommandHandler(
    UserSessionsService service,
    IGetUserHandle getUser,
    IStringHashAlgorithm hash
) : ICommandHandler<AuthenticateCommand, Status<UserSession>>
{
    public async Task<Status<UserSession>> Handle(
        AuthenticateCommand command,
        CancellationToken ct = default
    )
    {
        var user = await getUser.Get(command.Login, command.Email, ct);
        if (user.IsFailure)
            return Error.NotFound("Пользователь не найден.");

        var password = UserPassword.Create(command.Password);
        if (password.IsFailure)
            return Error.Unauthorized();

        var confirmation = user.Value.Verify(password, hash);
        return confirmation.IsFailure ? Error.Unauthorized() : await service.Create(user);
    }
}
