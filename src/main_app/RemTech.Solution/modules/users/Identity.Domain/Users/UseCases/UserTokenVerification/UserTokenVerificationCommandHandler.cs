using Identity.Domain.Tokens;
using Identity.Domain.Tokens.Ports;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserTokenVerification;

public sealed class UserTokenVerificationCommandHandler(ITokensStorage tokens)
    : ICommandHandler<UserTokenVerificationCommand, Status<UserToken>>
{
    public async Task<Status<UserToken>> Handle(
        UserTokenVerificationCommand command,
        CancellationToken ct = default
    )
    {
        Token token = Token.Create(command.TokenId);
        UserToken? required = await tokens.Get(token, ct);
        return required == null ? new Error("не авторизован", ErrorCodes.Unauthorized) : required;
    }
}
