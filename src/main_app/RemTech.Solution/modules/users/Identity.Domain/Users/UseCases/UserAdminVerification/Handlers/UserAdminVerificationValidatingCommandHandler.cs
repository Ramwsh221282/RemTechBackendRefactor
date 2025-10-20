using Identity.Domain.Tokens;
using Identity.Domain.Users.UseCases.UserAdminVerification.Input;
using Identity.Domain.Users.UseCases.UserAdminVerification.Output;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserAdminVerification.Handlers;

public sealed class UserAdminVerificationValidatingCommandHandler(
    ICommandHandler<UserAdminVerificationCommand, Status<UserAdminVerificationResponse>> handler
) : ICommandHandler<UserAdminVerificationCommand, Status<UserAdminVerificationResponse>>
{
    public async Task<Status<UserAdminVerificationResponse>> Handle(
        UserAdminVerificationCommand command,
        CancellationToken ct = default
    )
    {
        ValidationScope scope = new ValidationScope().Check(Token.Create(command.TokenId));
        return scope.Any()
            ? scope.ToError()
            : await handler.Handle(new UserAdminVerificationCommand(command.TokenId), ct);
    }
}
