using Shared.Infrastructure.Module.Cqrs;

namespace Users.Module.Features.UserPasswordRecovering.Interactor;

internal sealed class UserPasswordRecoveringChain : ICommandHandler<UserPasswordRecoveringContext>
{
    private readonly List<ICommandHandler<UserPasswordRecoveringContext>> _nodes = [];

    public UserPasswordRecoveringChain AddNode(ICommandHandler<UserPasswordRecoveringContext> node)
    {
        _nodes.Add(node);
        return this;
    }

    public async Task Handle(UserPasswordRecoveringContext command, CancellationToken ct = default)
    {
        foreach (ICommandHandler<UserPasswordRecoveringContext> node in _nodes)
            await node.Handle(command, ct);
    }
}
