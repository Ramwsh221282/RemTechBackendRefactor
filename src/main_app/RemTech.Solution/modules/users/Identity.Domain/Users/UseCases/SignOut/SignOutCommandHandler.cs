using Identity.Domain.Tokens;
using Identity.Domain.Tokens.Ports;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.SignOut;

public sealed class SignOutCommandHandler(ITokensStorage storage)
    : ICommandHandler<SignOutCommand, Status>
{
    public async Task<Status> Handle(SignOutCommand command, CancellationToken ct = default)
    {
        Token token = Token.Create(command.TokenId);
        await storage.Remove(token, ct);
        return Status.Success();
    }
}
