using Identity.Domain.Tokens;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.SignOut.Handlers;

public sealed class SignOutValidatingCommandHandler(ICommandHandler<SignOutCommand, Status> handler)
    : ICommandHandler<SignOutCommand, Status>
{
    public async Task<Status> Handle(SignOutCommand command, CancellationToken ct = default)
    {
        ValidationScope scope = new ValidationScope().Check(Token.Create(command.TokenId));
        return scope.Any() ? scope.ToError().Status() : await handler.Handle(command, ct);
    }
}
