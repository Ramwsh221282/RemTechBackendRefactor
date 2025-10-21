using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.AdminCreatesUser.Decorators;

public sealed class AdminCreatesUserTransactionalCommandHandler(
    IIdentityTransactionManager transactionManager,
    ICommandHandler<AdminCreatesUserCommand, Status<User>> handler
) : ICommandHandler<AdminCreatesUserCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        AdminCreatesUserCommand command,
        CancellationToken ct = default
    )
    {
        await using IIdentityTransactionScope scope = await transactionManager.BeginTransaction(ct);

        Status<User> created = await handler.Handle(command, ct);
        if (created.IsFailure)
            return created.Error;

        Status commit = await scope.Commit(ct);
        if (commit.IsFailure)
            return commit.Error;

        return created;
    }
}
