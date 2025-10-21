using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserRegistration.Decorators;

public sealed class UserRegistrationValidatingCommandHandler(
    ICommandHandler<UserRegistrationCommand, Status<User>> handler
) : ICommandHandler<UserRegistrationCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
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
