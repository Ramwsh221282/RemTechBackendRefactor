using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.CreateRoot.Decorators;

public sealed class CreateRootUserTransactionalCommandHandler(
    IIdentityTransactionManager transactionManager,
    ICommandHandler<CreateRootUserCommand, Status<User>> handler
) : ICommandHandler<CreateRootUserCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        CreateRootUserCommand command,
        CancellationToken ct = default
    )
    {
        await using IIdentityTransactionScope scope = await transactionManager.BeginTransaction(ct);

        Status<User> root = await handler.Handle(command, ct);
        if (root.IsFailure)
            return root.Error;

        Status commit = await scope.Commit(ct);
        if (commit.IsFailure)
            return commit.Error;

        return root;
    }
}
