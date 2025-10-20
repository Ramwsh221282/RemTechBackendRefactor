using Identity.Domain.Users.UseCases.UserRegistration.Input;
using Identity.Domain.Users.UseCases.UserRegistration.Output;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserRegistration.Handlers;

public sealed class UserRegistrationValidatingCommandHandler(
    ICommandHandler<UserRegistrationCommand, Status<UserRegistrationResponse>> handler
) : ICommandHandler<UserRegistrationCommand, Status<UserRegistrationResponse>>
{
    public async Task<Status<UserRegistrationResponse>> Handle(
        UserRegistrationCommand command,
        CancellationToken ct = default
    )
    {
        ValidationScope scope = new ValidationScope()
            .Check(UserLogin.Create(command.UserLogin))
            .Check(UserEmail.Create(command.UserEmail))
            .Check(UserPassword.Create(command.UserPassword));
        return scope.Any() ? scope.ToError() : await handler.Handle(command, ct);
    }
}
