using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Tokens;
using Identity.Domain.Tokens.Ports;
using Identity.Domain.Users.UseCases.UserAdminVerification.Input;
using Identity.Domain.Users.UseCases.UserAdminVerification.Output;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserAdminVerification.Handlers;

public sealed class UserAdminVerificationCommandHandler(ITokensStorage tokens)
    : ICommandHandler<UserAdminVerificationCommand, Status<UserAdminVerificationResponse>>
{
    public async Task<Status<UserAdminVerificationResponse>> Handle(
        UserAdminVerificationCommand command,
        CancellationToken ct = default
    )
    {
        Token token = Token.Create(command.TokenId);
        RoleName role = RoleName.Admin;
        UserToken? required = await tokens.Get(token, role, ct);
        return required == null
            ? Error.Forbidden("Не является администратором.")
            : new UserAdminVerificationResponse(required);
    }
}
