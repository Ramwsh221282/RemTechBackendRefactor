using Shared.Infrastructure.Module.Cqrs;
using Users.Module.Features.UserPasswordRecovering.Core;

namespace Users.Module.Features.UserPasswordRecovering.Interactor;

internal sealed class UserPasswordRecoveringValidationNode
    : ICommandHandler<UserPasswordRecoveringContext>
{
    public Task Handle(UserPasswordRecoveringContext command, CancellationToken ct = default)
    {
        command.Print(out string? email, out string? password);
        command.Attach(new UserRecoveringDispatcher(email, password).Dispatch());
        return Task.CompletedTask;
    }
}
