using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.UseCases.UserRegistrationByAdmin.Input;
using Identity.Domain.Users.UseCases.UserRegistrationByAdmin.Output;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserRegistrationByAdmin.Handlers;

public sealed class UserRegistrationByAdminTransactionalCommandHandler(
    IIdentityTransactionManager transactionManager,
    ICommandHandler<UserRegistrationByAdminCommand, Status<UserRegistrationByAdminResponse>> handler
) : ICommandHandler<UserRegistrationByAdminCommand, Status<UserRegistrationByAdminResponse>>
{
    public async Task<Status<UserRegistrationByAdminResponse>> Handle(
        UserRegistrationByAdminCommand command,
        CancellationToken ct = default
    )
    {
        await using IIdentityTransactionScope txn = await transactionManager.BeginTransaction(ct);
        Status<UserRegistrationByAdminResponse> result = await handler.Handle(command, ct);
        if (result.IsFailure)
            return result.Error;

        Status commit = await txn.Commit(ct);
        if (commit.IsFailure)
            return commit.Error;

        return result;
    }
}
