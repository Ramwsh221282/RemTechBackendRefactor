using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserRegistration.Decorators;

public sealed class UserRegistrationTransactionalCommandHandler(
    IIdentityTransactionManager transactionManager,
    ICommandHandler<UserRegistrationCommand, Status<User>> handler
) : ICommandHandler<UserRegistrationCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        UserRegistrationCommand command,
        CancellationToken ct = default
    )
    {
        await using IIdentityTransactionScope scope = await transactionManager.BeginTransaction(ct);
        Status<User> result = await handler.Handle(command, ct);
        if (result.IsFailure)
            return result.Error;

        Status commit = await scope.Commit(ct);
        if (commit.IsFailure)
            return commit.Error;

        return result;
    }
}
