using Identity.Domain.Tokens;
using Identity.Domain.Tokens.Ports;
using Identity.Domain.Users.UseCases.UserTokenVerification.Input;
using Identity.Domain.Users.UseCases.UserTokenVerification.Output;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserTokenVerification.Handlers;

public sealed class UserTokenVerificationCommandHandler(ITokensStorage tokens)
    : ICommandHandler<UserTokenVerificationCommand, Status<UserTokenVerificationResponse>>
{
    public async Task<Status<UserTokenVerificationResponse>> Handle(
        UserTokenVerificationCommand command,
        CancellationToken ct = default
    )
    {
        Token token = Token.Create(command.TokenId);
        UserToken? required = await tokens.Get(token, ct);
        return required == null
            ? new Error("не авторизован", ErrorCodes.Unauthorized)
            : new UserTokenVerificationResponse(required);
    }
}
