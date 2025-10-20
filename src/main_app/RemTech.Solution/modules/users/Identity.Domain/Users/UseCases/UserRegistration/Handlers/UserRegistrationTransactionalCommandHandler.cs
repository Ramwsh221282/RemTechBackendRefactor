using Identity.Domain.Users.Ports;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.UseCases.UserRegistration.Input;
using Identity.Domain.Users.UseCases.UserRegistration.Output;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserRegistration.Handlers;

public sealed class UserRegistrationTransactionalCommandHandler(
    IIdentityTransactionManager transactionManager,
    ICommandHandler<UserRegistrationCommand, Status<UserRegistrationResponse>> handler
) : ICommandHandler<UserRegistrationCommand, Status<UserRegistrationResponse>>
{
    public async Task<Status<UserRegistrationResponse>> Handle(
        UserRegistrationCommand command,
        CancellationToken ct = default
    )
    {
        await using IIdentityTransactionScope scope = await transactionManager.BeginTransaction(ct);
        Status<UserRegistrationResponse> result = await handler.Handle(command, ct);
        if (result.IsFailure)
            return result.Error;

        Status commit = await scope.Commit(ct);
        if (commit.IsFailure)
            return commit.Error;

        return result;
    }
}
