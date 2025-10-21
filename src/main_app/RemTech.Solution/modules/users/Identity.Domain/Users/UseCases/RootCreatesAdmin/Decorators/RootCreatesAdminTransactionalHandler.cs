using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.RootCreatesAdmin.Decorators;

public sealed class RootCreatesAdminTransactionalHandler(
    IIdentityTransactionManager transactionManager,
    ICommandHandler<RootCreatesAdminCommand, Status<User>> handler
) : ICommandHandler<RootCreatesAdminCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        RootCreatesAdminCommand command,
        CancellationToken ct = default
    )
    {
        await using IIdentityTransactionScope scope = await transactionManager.BeginTransaction(ct);

        Status<User> created = await handler.Handle(command, ct);
        if (created.IsFailure)
            return created.Error;

        Status commit = await scope.Commit(ct);
        return commit.IsFailure ? commit.Error : commit;
    }
}
