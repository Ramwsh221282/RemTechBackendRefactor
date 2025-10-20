using Identity.Domain.Tokens;
using Identity.Domain.Users.UseCases.UserTokenVerification.Input;
using Identity.Domain.Users.UseCases.UserTokenVerification.Output;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserTokenVerification.Handlers;

public sealed class UserTokenVerificationValidatingCommandHandler(
    ICommandHandler<UserTokenVerificationCommand, Status<UserTokenVerificationResponse>> handler
) : ICommandHandler<UserTokenVerificationCommand, Status<UserTokenVerificationResponse>>
{
    public async Task<Status<UserTokenVerificationResponse>> Handle(
        UserTokenVerificationCommand command,
        CancellationToken ct = default
    )
    {
        ValidationScope scope = new ValidationScope().Check(Token.Create(command.TokenId));
        return scope.Any() ? scope.ToError() : await handler.Handle(command, ct);
    }
}
